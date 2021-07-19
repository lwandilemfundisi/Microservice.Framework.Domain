using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microservice.Framework.Common;
using Microservice.Framework.Domain.Events.Serializers;
using Microservice.Framework.Domain.Exceptions;
using Microservice.Framework.Domain.ExecutionResults;
using Microservice.Framework.Domain.Subscribers;
using Microservice.Framework.Ioc;
using Microservice.Framework.Persistence;
using Microservice.Framework.Persistence.Extensions;
using Microservice.Framework.Persistence.Queries.Filtering;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Aggregates
{
    public class AggregateStore : IAggregateStore
    {
        private readonly ILogger<AggregateStore> _logger;        
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventJsonSerializer _eventJsonSerializer;
        private readonly IPersistenceFactory _persistenceFactory;
        private readonly IAggregateFactory _aggregateFactory;
        private readonly ITransientFaultHandler<IOptimisticConcurrencyResilientStrategy> _transientFaultHandler;
        private readonly ICancellationConfiguration _cancellationConfiguration;

        public AggregateStore(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<AggregateStore>>();
            _eventJsonSerializer = serviceProvider.GetRequiredService<IEventJsonSerializer>();
            _persistenceFactory = serviceProvider.GetRequiredService<IPersistenceFactory>();
            _aggregateFactory = serviceProvider.GetRequiredService<IAggregateFactory>();
            _transientFaultHandler = serviceProvider.GetRequiredService<ITransientFaultHandler<IOptimisticConcurrencyResilientStrategy>>();
            _cancellationConfiguration = serviceProvider.GetRequiredService<ICancellationConfiguration>();
        }

        public async Task<TAggregate> LoadAsync<TAggregate, TIdentity>(
            TIdentity id, 
            CancellationToken cancellationToken)
            where TAggregate : class, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            TAggregate aggregate = default;

            var criteria = new DomainCriteria();
            criteria.SafeAnd(new EqualityFilter("Id", id));
            IPersistence aggregatePersistence = _persistenceFactory.GetPersistence<TAggregate>();
            aggregate = await aggregatePersistence.Get<TAggregate, DomainCriteria>(criteria, CancellationToken.None);

            if(aggregate.IsNull())
            {
                aggregate = await _aggregateFactory.CreateNewAggregateAsync<TAggregate, TIdentity>(id).ConfigureAwait(false);
                aggregate.Persistence = aggregatePersistence;
                return aggregate;
            }
            else
            {
                aggregate.Persistence = aggregatePersistence;
                return aggregate.AsExisting() as TAggregate;
            }
        }

        public async Task StoreAsync<TAggregate, TIdentity>(
            TAggregate aggregate, 
            ISourceId sourceId, 
            CancellationToken cancellationToken)
            where TAggregate : class, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            await aggregate.CommitAsync(
                _eventJsonSerializer,
                sourceId,
                cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync<TAggregate, TIdentity>(
            TIdentity id, 
            ISourceId sourceId, 
            Func<TAggregate, CancellationToken, Task> updateAggregate, 
            CancellationToken cancellationToken)
            where TAggregate : class, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateUpdateResult = await UpdateAsync<TAggregate, TIdentity, IExecutionResult>(
                id,
                sourceId,
                async (a, c) =>
                {
                    await updateAggregate(a, c).ConfigureAwait(false);
                    return ExecutionResult.Success();
                },
                cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IAggregateUpdateResult<TExecutionResult>> UpdateAsync<TAggregate, TIdentity, TExecutionResult>(
            TIdentity id, 
            ISourceId sourceId, 
            Func<TAggregate, CancellationToken, Task<TExecutionResult>> updateAggregate, 
            CancellationToken cancellationToken)
            where TAggregate : class, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
            where TExecutionResult : IExecutionResult
        {
            var aggregateUpdateResult = await _transientFaultHandler.TryAsync(
                async c =>
                {
                    var aggregate = await LoadAsync<TAggregate, TIdentity>(id, c).ConfigureAwait(false);
                    if (aggregate.HasSourceId(sourceId))
                    {
                        throw new DuplicateOperationException(
                            sourceId,
                            id,
                            $"Aggregate '{typeof(TAggregate).PrettyPrint()}' has already had operation '{sourceId}' performed");
                    }

                    cancellationToken = _cancellationConfiguration.Limit(cancellationToken, CancellationBoundary.BeforeUpdatingAggregate);

                    var result = await updateAggregate(aggregate, c).ConfigureAwait(false);
                    if (!result.IsSuccess)
                    {
                        _logger.LogDebug(
                            "Execution failed on aggregate {AggregateType}, disregarding any events emitted",
                            typeof(TAggregate).PrettyPrint());
                        return new AggregateUpdateResult<TExecutionResult>(result, null);
                    }

                    cancellationToken = _cancellationConfiguration.Limit(cancellationToken, CancellationBoundary.BeforeCommittingEvents);

                    var domainEvents = await aggregate.CommitAsync(
                        _eventJsonSerializer,
                        sourceId,
                        cancellationToken)
                        .ConfigureAwait(false);

                    return new AggregateUpdateResult<TExecutionResult>(result, domainEvents);
                },
                Label.Named("aggregate-update"),
                cancellationToken)
                .ConfigureAwait(false);

            if (aggregateUpdateResult.Result.IsSuccess &&
                aggregateUpdateResult.DomainEvents.Any())
            {
                var domainEventPublisher = _serviceProvider.GetRequiredService<IDomainEventPublisher>();
                await domainEventPublisher.PublishAsync(
                    aggregateUpdateResult.DomainEvents,
                    cancellationToken)
                    .ConfigureAwait(false);
            }

            return aggregateUpdateResult;
        }
    }
}

using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events;
using Microservice.Framework.Domain.Jobs;
using Microservice.Framework.Domain.Provided.Jobs;
using Microservice.Framework.Common;
using Microservice.Framework.Ioc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Microservice.Framework.Domain.Subscribers
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IDispatchToEventSubscribers _dispatchToEventSubscribers;
        private readonly IJobScheduler _jobScheduler;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISetup _setup;
        private readonly ICancellationConfiguration _cancellationConfiguration;
        private readonly IReadOnlyCollection<ISubscribeSynchronousToAll> _subscribeSynchronousToAlls;

        public DomainEventPublisher(
            IDispatchToEventSubscribers dispatchToEventSubscribers,
            IJobScheduler jobScheduler,
            IServiceProvider serviceProvider,
            ISetup setup,
            IEnumerable<ISubscribeSynchronousToAll> subscribeSynchronousToAlls,
            ICancellationConfiguration cancellationConfiguration)
        {
            _dispatchToEventSubscribers = dispatchToEventSubscribers;
            _jobScheduler = jobScheduler;
            _serviceProvider = serviceProvider;
            _setup = setup;
            _cancellationConfiguration = cancellationConfiguration;
            _subscribeSynchronousToAlls = subscribeSynchronousToAlls.ToList();
        }

        public Task PublishAsync<TAggregate, TIdentity>(
            TIdentity id,
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return PublishAsync(
                domainEvents,
                cancellationToken);
        }

        public async Task PublishAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
        {
            cancellationToken = _cancellationConfiguration.Limit(cancellationToken, CancellationBoundary.BeforeNotifyingSubscribers);
            await PublishToSubscribersOfAllEventsAsync(domainEvents, cancellationToken).ConfigureAwait(false);

            // Update subscriptions AFTER read stores have been updated
            await PublishToSynchronousSubscribersAsync(domainEvents, cancellationToken).ConfigureAwait(false);
            await PublishToAsynchronousSubscribersAsync(domainEvents, cancellationToken).ConfigureAwait(false);
        }

        private async Task PublishToSubscribersOfAllEventsAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
        {
            var handle = _subscribeSynchronousToAlls
                .Select(s => s.HandleAsync(domainEvents, cancellationToken));
            await Task.WhenAll(handle).ConfigureAwait(false);
        }

        private async Task PublishToSynchronousSubscribersAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
        {
            await _dispatchToEventSubscribers.DispatchToSynchronousSubscribersAsync(domainEvents, cancellationToken).ConfigureAwait(false);
        }

        private async Task PublishToAsynchronousSubscribersAsync(
            IEnumerable<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
        {
            if (_setup.IsAsynchronousSubscribersEnabled)
            {
                await Task.WhenAll(domainEvents.Select(
                        d => _jobScheduler.ScheduleNowAsync(
                            DispatchToAsynchronousEventSubscribersJob.Create(d, _serviceProvider), cancellationToken)))
                    .ConfigureAwait(false);
            }
        }
    }
}
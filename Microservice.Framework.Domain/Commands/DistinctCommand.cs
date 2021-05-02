using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.ExecutionResults;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Commands
{
    public abstract class DistinctCommand<TAggregate, TIdentity, TExecutionResult> : ICommand<TAggregate, TIdentity, TExecutionResult>
        where TAggregate : class, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TExecutionResult : IExecutionResult
    {
        private readonly Lazy<ISourceId> _lazySourceId;

        public ISourceId SourceId => _lazySourceId.Value;
        public TIdentity AggregateId { get; }

        protected DistinctCommand(
            TIdentity aggregateId)
        {
            if (aggregateId == null) throw new ArgumentNullException(nameof(aggregateId));

            _lazySourceId = new Lazy<ISourceId>(CalculateSourceId, LazyThreadSafetyMode.PublicationOnly);

            AggregateId = aggregateId;
        }

        private CommandId CalculateSourceId()
        {
            var bytes = GetSourceIdComponents().SelectMany(b => b).ToArray();
            return CommandId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Commands,
                bytes);
        }

        protected abstract IEnumerable<byte[]> GetSourceIdComponents();

        public async Task<IExecutionResult> PublishAsync(ICommandBus commandBus, CancellationToken cancellationToken)
        {
            return await commandBus.PublishAsync(this, cancellationToken).ConfigureAwait(false);
        }

        public ISourceId GetSourceId()
        {
            return SourceId;
        }
    }
}

using Microservice.Framework.Domain.Events;
using Microservice.Framework.Domain.ExecutionResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Aggregates
{
    public class AggregateUpdateResult<TExecutionResult> : IAggregateUpdateResult<TExecutionResult>
            where TExecutionResult : IExecutionResult
    {
        public TExecutionResult Result { get; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        public AggregateUpdateResult(
            TExecutionResult result,
            IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            Result = result;
            DomainEvents = domainEvents;
        }
    }
}

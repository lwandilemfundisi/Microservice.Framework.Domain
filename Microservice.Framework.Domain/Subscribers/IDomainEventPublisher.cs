using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Subscribers
{
    public interface IDomainEventPublisher
    {
        Task PublishAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken);

        [Obsolete("Use PublishAsync (without generics and aggregate identity)")]
        Task PublishAsync<TAggregate, TIdentity>(
            TIdentity id,
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity;
    }
}
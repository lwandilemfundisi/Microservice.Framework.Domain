using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events;
using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Subscribers
{
    public interface ISubscribeAsynchronousTo<TAggregate, in TIdentity, in TEvent>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TEvent : IAggregateEvent<TAggregate, TIdentity>
    {
        Task HandleAsync(IDomainEvent<TAggregate, TIdentity, TEvent> domainEvent, CancellationToken cancellationToken);
    }
}
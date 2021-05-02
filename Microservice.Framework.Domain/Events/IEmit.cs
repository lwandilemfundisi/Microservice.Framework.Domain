using Microservice.Framework.Domain.Events.AggregateEvents;

namespace Microservice.Framework.Domain.Events
{
    public interface IEmit<in TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        void Apply(TAggregateEvent aggregateEvent);
    }
}
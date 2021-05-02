using Microservice.Framework.Domain.Events.AggregateEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events
{
    public class OccuredEvent : IOccuredEvent
    {
        public IAggregateEvent AggregateEvent { get; }

        public IMetadata Metadata { get; }

        public OccuredEvent(IAggregateEvent aggregateEvent, IMetadata metadata)
        {
            AggregateEvent = aggregateEvent;
            Metadata = metadata;
        }
    }
}

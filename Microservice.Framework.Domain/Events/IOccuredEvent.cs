using Microservice.Framework.Domain.Events.AggregateEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events
{
    public interface IOccuredEvent
    {
        IAggregateEvent AggregateEvent { get; }
        IMetadata Metadata { get; }
    }
}

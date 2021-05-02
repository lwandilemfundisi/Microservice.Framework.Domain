using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Common;
using Microservice.Framework.VersionTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events.AggregateEvents
{
    public interface IAggregateEvent : IVersionedType
    {
    }

    public interface IAggregateEvent<TAggregate, TIdentity> : IAggregateEvent
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}

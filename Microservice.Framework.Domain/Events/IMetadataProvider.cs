using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Common;
using System.Collections.Generic;

namespace Microservice.Framework.Domain.Events
{
    public interface IMetadataProvider
    {
        IEnumerable<KeyValuePair<string, string>> ProvideMetadata<TAggregate, TIdentity>(
            TIdentity id,
            IAggregateEvent aggregateEvent,
            IMetadata metadata)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity;
    }
}
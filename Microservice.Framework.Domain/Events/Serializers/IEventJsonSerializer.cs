using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events.Serializers
{
    public interface IEventJsonSerializer
    {
        SerializedEvent Serialize(
            IDomainEvent domainEvent);

        SerializedEvent Serialize(
            IAggregateEvent aggregateEvent,
            IEnumerable<KeyValuePair<string, string>> metadatas);

        IDomainEvent Deserialize(string json, IMetadata metadata);

        IDomainEvent Deserialize(ICommittedDomainEvent committedDomainEvent);

        IDomainEvent<TAggregate, TIdentity> Deserialize<TAggregate, TIdentity>(
            TIdentity id,
            ICommittedDomainEvent committedDomainEvent)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity;

        IDomainEvent Deserialize(string eventJson, string metadataJson);
    }
}

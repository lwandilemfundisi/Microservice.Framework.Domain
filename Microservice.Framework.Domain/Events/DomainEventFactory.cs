using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Common;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Microservice.Framework.Domain.Events
{
    public class DomainEventFactory : IDomainEventFactory
    {
        private static readonly ConcurrentDictionary<Type, Type> AggregateEventToDomainEventTypeMap = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, Type> DomainEventToIdentityTypeMap = new ConcurrentDictionary<Type, Type>();

        public IDomainEvent Create(
            IAggregateEvent aggregateEvent,
            IMetadata metadata,
            string aggregateIdentity,
            int aggregateSequenceNumber)
        {
            var domainEventType = AggregateEventToDomainEventTypeMap.GetOrAdd(aggregateEvent.GetType(), GetDomainEventType);
            var identityType = DomainEventToIdentityTypeMap.GetOrAdd(domainEventType, GetIdentityType);
            var identity = Activator.CreateInstance(identityType, aggregateIdentity);

            var domainEvent = (IDomainEvent)Activator.CreateInstance(
                domainEventType,
                aggregateEvent,
                metadata,
                metadata.Timestamp,
                identity,
                aggregateSequenceNumber);

            return domainEvent;
        }

        public IDomainEvent<TAggregate, TIdentity> Create<TAggregate, TIdentity>(
            IAggregateEvent aggregateEvent,
            IMetadata metadata,
            TIdentity id,
            int aggregateSequenceNumber)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return (IDomainEvent<TAggregate, TIdentity>)Create(
                aggregateEvent,
                metadata,
                id.Value,
                aggregateSequenceNumber);
        }

        public IDomainEvent<TAggregate, TIdentity> Upgrade<TAggregate, TIdentity>(
            IDomainEvent domainEvent,
            IAggregateEvent aggregateEvent)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return Create<TAggregate, TIdentity>(
                aggregateEvent,
                domainEvent.Metadata,
                (TIdentity) domainEvent.GetIdentity(),
                domainEvent.AggregateSequenceNumber);
        }

        private static Type GetIdentityType(Type domainEventType)
        {
            var domainEventInterfaceType = domainEventType
                .GetTypeInfo()
                .GetInterfaces()
                .SingleOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEvent<,>));

            if (domainEventInterfaceType == null)
            {
                throw new ArgumentException($"Type '{domainEventType.PrettyPrint()}' is not a '{typeof(IDomainEvent<,>).PrettyPrint()}'");
            }

            var genericArguments = domainEventInterfaceType.GetTypeInfo().GetGenericArguments();
            return genericArguments[1];
        }

        private static Type GetDomainEventType(Type aggregateEventType)
        {
            var aggregateEventInterfaceType = aggregateEventType
                .GetTypeInfo()
                .GetInterfaces()
                .SingleOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IAggregateEvent<,>));

            if (aggregateEventInterfaceType == null)
            {
                throw new ArgumentException($"Type '{aggregateEventType.PrettyPrint()}' is not a '{typeof(IAggregateEvent<,>).PrettyPrint()}'");
            }

            var genericArguments = aggregateEventInterfaceType.GetTypeInfo().GetGenericArguments();
            return typeof(DomainEvent<,,>).MakeGenericType(genericArguments[0], genericArguments[1], aggregateEventType);
        }
    }
}

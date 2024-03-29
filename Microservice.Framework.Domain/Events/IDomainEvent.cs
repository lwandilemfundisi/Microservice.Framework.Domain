﻿using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Common;
using System;

namespace Microservice.Framework.Domain.Events
{
    public interface IDomainEvent
    {
        Type AggregateType { get; }
        Type IdentityType { get; }
        Type EventType { get; }
        int AggregateSequenceNumber { get; }
        IMetadata Metadata { get; }
        DateTimeOffset Timestamp { get; }

        IIdentity GetIdentity();
        IAggregateEvent GetAggregateEvent();
    }

    public interface IDomainEvent<TAggregate, out TIdentity> : IDomainEvent
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        TIdentity AggregateIdentity { get; }
    }

    public interface IDomainEvent<TAggregate, out TIdentity, out TAggregateEvent> : IDomainEvent<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
    {
        TAggregateEvent AggregateEvent { get; }
    }
}
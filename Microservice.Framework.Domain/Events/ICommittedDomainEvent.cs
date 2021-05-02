using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events
{
    public interface ICommittedDomainEvent
    {
        string AggregateId { get; }
        string Data { get; }
        string Metadata { get; }
        int AggregateSequenceNumber { get; }
    }
}

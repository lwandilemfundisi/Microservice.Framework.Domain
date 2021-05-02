using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events.Serializers
{
    public interface ISerializedEvent
    {
        string SerializedMetadata { get; }
        string SerializedData { get; }
        int AggregateSequenceNumber { get; }
        IMetadata Metadata { get; }
    }
}

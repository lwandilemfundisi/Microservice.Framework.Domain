using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events.Serializers
{
    public class SerializedEvent : ISerializedEvent
    {
        public string SerializedMetadata { get; }
        public string SerializedData { get; }
        public int AggregateSequenceNumber { get; }
        public IMetadata Metadata { get; }

        public SerializedEvent(
            string serializedMetadata,
            string serializedData,
            int aggregateSequenceNumber,
            IMetadata metadata)
        {
            SerializedMetadata = serializedMetadata;
            SerializedData = serializedData;
            AggregateSequenceNumber = aggregateSequenceNumber;
            Metadata = metadata;
        }
    }
}

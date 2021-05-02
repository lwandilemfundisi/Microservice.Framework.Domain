using Microservice.Framework.VersionTypes;
using System;

namespace Microservice.Framework.Domain.Events
{
    public class EventDefinition : VersionedTypeDefinition
    {
        public EventDefinition(
            int version,
            Type type,
            string name)
            : base(version, type, name)
        {
        }
    }
}
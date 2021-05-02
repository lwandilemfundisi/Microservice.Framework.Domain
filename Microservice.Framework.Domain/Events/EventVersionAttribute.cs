using Microservice.Framework.VersionTypes;
using System;

namespace Microservice.Framework.Domain.Events
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = true
        )]
    public class EventVersionAttribute : VersionedTypeAttribute
    {
        public EventVersionAttribute(string name, int version)
            : base(name, version)
        {
        }
    }
}
using Microservice.Framework.VersionTypes;
using System;

namespace Microservice.Framework.Domain.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobVersionAttribute : VersionedTypeAttribute
    {
        public JobVersionAttribute(
            string name,
            int version)
            : base(name, version)
        {
        }
    }
}
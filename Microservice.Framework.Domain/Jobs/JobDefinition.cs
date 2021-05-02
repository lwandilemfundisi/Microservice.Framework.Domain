using Microservice.Framework.VersionTypes;
using System;

namespace Microservice.Framework.Domain.Jobs
{
    public class JobDefinition : VersionedTypeDefinition
    {
        public JobDefinition(
            int version,
            Type type,
            string name)
            : base(version, type, name)
        {
        }
    }
}
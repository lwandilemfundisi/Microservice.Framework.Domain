using Microsoft.Extensions.Logging;
using Microservice.Framework.Common;
using Microservice.Framework.Ioc;
using Microservice.Framework.VersionTypes;
using System;
using System.Linq;
using System.Reflection;

namespace Microservice.Framework.Domain.Jobs
{
    public class JobDefinitionService : VersionedTypeDefinitionService<IJob, JobVersionAttribute, JobDefinition>, IJobDefinitionService
    {
        public JobDefinitionService(
            ILogger<JobDefinitionService> logger,
            ILoadedTypes loadedTypes)
            : base(logger)
        {
            var jobTypes = loadedTypes
                .TypesLoaded
                .Where(t => typeof(IJob).GetTypeInfo().IsAssignableFrom(t));
            Load(jobTypes.ToArray());
        }

        protected override JobDefinition CreateDefinition(int version, Type type, string name)
        {
            return new JobDefinition(version, type, name);
        }
    }
}
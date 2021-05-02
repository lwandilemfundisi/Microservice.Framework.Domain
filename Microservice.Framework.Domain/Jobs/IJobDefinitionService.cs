using Microservice.Framework.VersionTypes;

namespace Microservice.Framework.Domain.Jobs
{
    public interface IJobDefinitionService : IVersionedTypeDefinitionService<JobVersionAttribute, JobDefinition>
    {
    }
}
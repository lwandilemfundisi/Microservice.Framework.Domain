using Microservice.Framework.VersionTypes;

namespace Microservice.Framework.Domain.Events
{
    public interface IEventDefinitionService : IVersionedTypeDefinitionService<EventVersionAttribute, EventDefinition>
    {
    }
}
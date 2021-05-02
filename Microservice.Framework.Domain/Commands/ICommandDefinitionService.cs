using Microservice.Framework.VersionTypes;

namespace Microservice.Framework.Domain.Commands
{
    public interface ICommandDefinitionService : IVersionedTypeDefinitionService<CommandVersionAttribute, CommandDefinition>
    {
    }
}

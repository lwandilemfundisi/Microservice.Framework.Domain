using Microsoft.Extensions.Logging;
using Microservice.Framework.Common;
using Microservice.Framework.Ioc;
using Microservice.Framework.VersionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microservice.Framework.Domain.Commands
{
    public class CommandDefinitionService : VersionedTypeDefinitionService<ICommand, CommandVersionAttribute, CommandDefinition>, ICommandDefinitionService
    {
        public CommandDefinitionService(
            ILogger<CommandDefinitionService> logger,
            ILoadedTypes loadedTypes) 
            : base(logger)
        {
            var commandTypes = loadedTypes
                .TypesLoaded
                .Where(t => typeof(ICommand).GetTypeInfo().IsAssignableFrom(t));
            Load(commandTypes.ToArray());
        }

        protected override CommandDefinition CreateDefinition(int version, Type type, string name)
        {
            return new CommandDefinition(version, type, name);
        }
    }
}

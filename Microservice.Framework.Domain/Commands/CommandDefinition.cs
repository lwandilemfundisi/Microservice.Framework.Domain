using Microservice.Framework.Common;
using Microservice.Framework.VersionTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Commands
{
    public class CommandDefinition : VersionedTypeDefinition
    {
        public CommandDefinition(
            int version,
            Type type,
            string name)
            : base(version, type, name)
        {
        }
    }
}

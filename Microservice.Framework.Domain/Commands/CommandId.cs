using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Commands
{
    public class CommandId : Identity<CommandId>, ICommandId
    {
        public CommandId(string value) : base(value)
        {
        }
    }
}

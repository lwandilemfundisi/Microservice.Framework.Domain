using Microservice.Framework.Common;
using Microservice.Framework.VersionTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandVersionAttribute : VersionedTypeAttribute
    {
        public CommandVersionAttribute(
            string name,
            int version)
            : base(name, version)
        {
        }
    }
}

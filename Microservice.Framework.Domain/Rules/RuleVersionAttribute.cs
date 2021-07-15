using Microservice.Framework.VersionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RuleVersionAttribute : VersionedTypeAttribute
    {
        public RuleVersionAttribute(
            string name,
            int version)
            : base(name, version)
        {
        }
    }
}

using Microservice.Framework.VersionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules
{
    public interface IRuleDefinitionService 
        : IVersionedTypeDefinitionService<RuleVersionAttribute, RuleDefinition>
    {
    }
}

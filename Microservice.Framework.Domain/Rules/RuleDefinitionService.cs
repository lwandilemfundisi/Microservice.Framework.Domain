using Microservice.Framework.Ioc;
using Microservice.Framework.VersionTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules
{
    public class RuleDefinitionService 
        : VersionedTypeDefinitionService<IRule, RuleVersionAttribute, RuleDefinition>, IRuleDefinitionService
    {
        public RuleDefinitionService(
            ILogger<RuleDefinitionService> logger,
            ILoadedTypes loadedTypes)
            : base(logger)
        {
            var jobTypes = loadedTypes
                .TypesLoaded
                .Where(t => typeof(IRule).GetTypeInfo().IsAssignableFrom(t));
            Load(jobTypes.ToArray());
        }

        protected override RuleDefinition CreateDefinition(int version, Type type, string name)
        {
            return new RuleDefinition(version, type, name);
        }
    }
}

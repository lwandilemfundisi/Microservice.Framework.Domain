using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerRulesExtensions
    {
        public static IDomainContainer AddRules(
            this IDomainContainer domainContainer,
            Assembly fromAssembly,
            Predicate<Type> predicate = null)
        {
            predicate = predicate ?? (t => true);
            var ruleTypes = fromAssembly
                .GetTypes()
                .Where(t => t.IsRule())
                .Where(t => predicate(t));

            foreach (var ruleType in ruleTypes)
            {
                domainContainer
                    .ServiceCollection
                    .AddTransient(ruleType);
            }

            return domainContainer
            .AddRules(ruleTypes);
        }

        private static bool IsRule(this Type type)
        {
            return typeof(IRule).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface;
        }
    }
}

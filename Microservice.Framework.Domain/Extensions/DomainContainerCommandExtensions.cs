using Microservice.Framework.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerCommandExtensions
    {
        public static IDomainContainer AddCommands(
            this IDomainContainer domainContainer,
            params Type[] commandTypes)
        {
            return domainContainer.AddCommands(commandTypes);
        }

        public static IDomainContainer AddCommands(
            this IDomainContainer domainContainer,
            Assembly fromAssembly,
            Predicate<Type> predicate)
        {
            predicate = predicate ?? (t => true);
            var commandTypes = fromAssembly
                .GetTypes()
                .Where(t => !t.GetTypeInfo().IsAbstract && typeof(ICommand).GetTypeInfo().IsAssignableFrom(t))
                .Where(t => predicate(t));
            return domainContainer.AddCommands(commandTypes);
        }
    }
}

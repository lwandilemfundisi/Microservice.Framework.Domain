using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerCommandHandlerExtensions
    {
        public static IDomainContainer AddCommandHandlers(
            this IDomainContainer domainContainer,
            Assembly fromAssembly,
            Predicate<Type> predicate = null)
        {
            predicate = predicate ?? (t => true);
            var commandHandlerTypes = fromAssembly
                .GetTypes()
                .Where(t => t.GetTypeInfo().GetInterfaces().Any(IsCommandHandlerInterface))
                .Where(t => !t.HasConstructorParameterOfType(IsCommandHandlerInterface))
                .Where(t => predicate(t));
            return domainContainer.AddCommandHandlers(commandHandlerTypes);
        }

        public static IDomainContainer AddCommandHandlers(
            this IDomainContainer domainContainer,
            params Type[] commandHandlerTypes)
        {
            return domainContainer.AddCommandHandlers((IEnumerable<Type>)commandHandlerTypes);
        }

        public static IDomainContainer AddCommandHandlers(
            this IDomainContainer domainContainer,
            IEnumerable<Type> commandHandlerTypes)
        {
            foreach (var commandHandlerType in commandHandlerTypes)
            {
                var t = commandHandlerType;
                if (t.GetTypeInfo().IsAbstract) continue;
                var handlesCommandTypes = t
                    .GetTypeInfo()
                    .GetInterfaces()
                    .Where(IsCommandHandlerInterface)
                    .ToList();
                if (!handlesCommandTypes.Any())
                {
                    throw new ArgumentException($"Type '{commandHandlerType.PrettyPrint()}' does not implement '{typeof(ICommandHandler<,,,>).PrettyPrint()}'");
                }

                foreach (var handlesCommandType in handlesCommandTypes)
                {
                    domainContainer.ServiceCollection.AddTransient(handlesCommandType, t);
                }
            }

            return domainContainer;
        }

        private static bool IsCommandHandlerInterface(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ICommandHandler<,,,>);
        }
    }
}

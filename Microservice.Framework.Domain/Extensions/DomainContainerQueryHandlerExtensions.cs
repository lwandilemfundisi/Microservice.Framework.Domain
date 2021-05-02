using Microservice.Framework.Domain.Queries;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerQueryHandlerExtensions
    {
        public static IDomainContainer AddQueryHandler<TQueryHandler, TQuery, TResult>(
            this IDomainContainer domainContainer)
            where TQueryHandler : class, IQueryHandler<TQuery, TResult>
            where TQuery : IQuery<TResult>
        {
            domainContainer.ServiceCollection
                .AddTransient<IQueryHandler<TQuery, TResult>, TQueryHandler>();
            return domainContainer;
        }

        public static IDomainContainer AddQueryHandlers(
            this IDomainContainer domainContainer,
            params Type[] queryHandlerTypes)
        {
            return domainContainer.AddQueryHandlers((IEnumerable<Type>)queryHandlerTypes);
        }

        public static IDomainContainer AddQueryHandlers(
            this IDomainContainer domainContainer,
            Assembly fromAssembly,
            Predicate<Type> predicate = null)
        {
            predicate = predicate ?? (t => true);
            var subscribeSynchronousToTypes = fromAssembly
                .GetTypes()
                .Where(t => t.GetTypeInfo().GetInterfaces().Any(IsQueryHandlerInterface))
                .Where(t => !t.HasConstructorParameterOfType(IsQueryHandlerInterface))
                .Where(t => predicate(t));
            return domainContainer
                .AddQueryHandlers(subscribeSynchronousToTypes);
        }

        public static IDomainContainer AddQueryHandlers(
            this IDomainContainer domainContainer,
            IEnumerable<Type> queryHandlerTypes)
        {
            foreach (var queryHandlerType in queryHandlerTypes)
            {
                var t = queryHandlerType;
                if (t.GetTypeInfo().IsAbstract) continue;
                var queryHandlerInterfaces = t
                    .GetTypeInfo()
                    .GetInterfaces()
                    .Where(IsQueryHandlerInterface)
                    .ToList();
                if (!queryHandlerInterfaces.Any())
                {
                    throw new ArgumentException($"Type '{t.PrettyPrint()}' is not an '{typeof(IQueryHandler<,>).PrettyPrint()}'");
                }

                foreach (var queryHandlerInterface in queryHandlerInterfaces)
                {
                    domainContainer.ServiceCollection.AddTransient(queryHandlerInterface, t);
                }
            }

            return domainContainer;
        }

        private static bool IsQueryHandlerInterface(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>);
        }
    }
}

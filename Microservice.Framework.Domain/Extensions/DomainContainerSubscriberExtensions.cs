using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Domain.Subscribers;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerSubscriberExtensions
    {
        private static readonly Type ISubscribeSynchronousToType = typeof(ISubscribeSynchronousTo<,,>);
        private static readonly Type ISubscribeAsynchronousToType = typeof(ISubscribeAsynchronousTo<,,>);
        private static readonly Type ISubscribeSynchronousToAllType = typeof(ISubscribeSynchronousToAll);

        [Obsolete("Please use the more explicit method 'AddSynchronousSubscriber<,,,>' instead")]
        public static IDomainContainer AddSubscriber<TAggregate, TIdentity, TEvent, TSubscriber>(
            this IDomainContainer domainContainer)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
            where TEvent : IAggregateEvent<TAggregate, TIdentity>
            where TSubscriber : class, ISubscribeSynchronousTo<TAggregate, TIdentity, TEvent>
        {
            domainContainer.ServiceCollection
                .AddTransient<ISubscribeSynchronousTo<TAggregate, TIdentity, TEvent>, TSubscriber>();
            return domainContainer;
        }

        public static IDomainContainer AddSynchronousSubscriber<TAggregate, TIdentity, TEvent, TSubscriber>(
            this IDomainContainer domainContainer)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
            where TEvent : IAggregateEvent<TAggregate, TIdentity>
            where TSubscriber : class, ISubscribeSynchronousTo<TAggregate, TIdentity, TEvent>
        {
            domainContainer.ServiceCollection
                .AddTransient<ISubscribeSynchronousTo<TAggregate, TIdentity, TEvent>, TSubscriber>();

            return domainContainer;
        }

        public static IDomainContainer AddAsynchronousSubscriber<TAggregate, TIdentity, TEvent, TSubscriber>(
            this IDomainContainer domainContainer)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
            where TEvent : IAggregateEvent<TAggregate, TIdentity>
            where TSubscriber : class, ISubscribeAsynchronousTo<TAggregate, TIdentity, TEvent>
        {
            domainContainer
                .ServiceCollection
                .AddTransient<ISubscribeAsynchronousTo<TAggregate, TIdentity, TEvent>, TSubscriber>();

            return domainContainer;
        }

        public static IDomainContainer AddSubscribers(
            this IDomainContainer domainContainer,
            params Type[] types)
        {
            return domainContainer.AddSubscribers((IEnumerable<Type>)types);
        }

        public static IDomainContainer AddSubscribers(
            this IDomainContainer domainContainer,
            Assembly fromAssembly,
            Predicate<Type> predicate = null)
        {
            predicate = predicate ?? (t => true);
            var types = fromAssembly
                .GetTypes()
                .Where(t => t.GetTypeInfo().GetInterfaces().Any(IsSubscriberInterface))
                .Where(t => !t.HasConstructorParameterOfType(IsSubscriberInterface))
                .Where(t => predicate(t));
            return domainContainer.AddSubscribers(types);
        }

        public static IDomainContainer AddSubscribers(
            this IDomainContainer domainContainer,
            IEnumerable<Type> subscribeSynchronousToTypes)
        {
            foreach (var subscribeSynchronousToType in subscribeSynchronousToTypes)
            {
                var t = subscribeSynchronousToType;
                if (t.GetTypeInfo().IsAbstract)
                {
                    continue;
                }

                var subscribeTos = t
                    .GetTypeInfo()
                    .GetInterfaces()
                    .Where(IsSubscriberInterface)
                    .ToList();
                if (!subscribeTos.Any())
                {
                    throw new ArgumentException($"Type '{t.PrettyPrint()}' is not an '{ISubscribeSynchronousToType.PrettyPrint()}', '{ISubscribeAsynchronousToType.PrettyPrint()}' or '{ISubscribeSynchronousToAllType.PrettyPrint()}'");
                }

                foreach (var subscribeTo in subscribeTos)
                {
                    domainContainer.ServiceCollection.AddTransient(subscribeTo, t);
                }
            }

            return domainContainer;
        }

        private static bool IsSubscriberInterface(Type type)
        {
            if (type == ISubscribeSynchronousToAllType)
            {
                return true;
            }

            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericType)
            {
                return false;
            }

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition == ISubscribeSynchronousToType ||
                   genericTypeDefinition == ISubscribeAsynchronousToType;
        }
    }
}

using Microservice.Framework.Domain.Events.AggregateEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerEventsExtensions
    {
        public static IDomainContainer AddEvents(
            this IDomainContainer domainContainer,
            Assembly fromAssembly,
            Predicate<Type> predicate = null)
        {
            predicate = predicate ?? (t => true);
            var aggregateEventTypes = fromAssembly
                .GetTypes()
                .Where(t => !t.GetTypeInfo().IsAbstract && typeof(IAggregateEvent).GetTypeInfo().IsAssignableFrom(t))
                .Where(t => predicate(t));
            return domainContainer.AddEvents(aggregateEventTypes);
        }

        public static IDomainContainer AddEvents(
            this IDomainContainer domainContainer,
            params Type[] aggregateEventTypes)
        {
            return domainContainer.AddEvents((IEnumerable<Type>)aggregateEventTypes);
        }
    }
}

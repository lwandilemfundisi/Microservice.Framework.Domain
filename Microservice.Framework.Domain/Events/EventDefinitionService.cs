using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Common;
using Microservice.Framework.VersionTypes;
using System;
using Microsoft.Extensions.Logging;
using Microservice.Framework.Ioc;
using System.Linq;
using System.Reflection;

namespace Microservice.Framework.Domain.Events
{
    public class EventDefinitionService : VersionedTypeDefinitionService<IAggregateEvent, EventVersionAttribute, EventDefinition>, IEventDefinitionService
    {
        public EventDefinitionService(
            ILogger<EventDefinitionService> logger,
            ILoadedTypes loadedTypes)
            : base(logger)
        {
            var eventTypes = loadedTypes
                .TypesLoaded
                .Where(t => typeof(IAggregateEvent).GetTypeInfo().IsAssignableFrom(t));
            Load(eventTypes.ToArray());
        }

        protected override EventDefinition CreateDefinition(int version, Type type, string name)
        {
            return new EventDefinition(version, type, name);
        }
    }
}
using Microservice.Framework.Common;
using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Domain.Events;
using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Domain.Events.Serializers;
using Microservice.Framework.Domain.Jobs;
using Microservice.Framework.Domain.Queries;
using Microservice.Framework.Domain.Rules;
using Microservice.Framework.Domain.Rules.RuleValidator;
using Microservice.Framework.Domain.Subscribers;
using Microservice.Framework.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microservice.Framework.Domain
{
    public class DomainContainer : Container, IDomainContainer
    {
        public DomainContainer(IServiceCollection serviceCollection) : base(serviceCollection)
        {
        }

        public static IDomainContainer New ()=> new DomainContainer(new ServiceCollection()
            .AddLogging(b => b.AddConsole()));

        public static IDomainContainer New(IServiceCollection serviceCollection) => new DomainContainer(serviceCollection);

        public IDomainContainer AddCommands(IEnumerable<Type> commandTypes)
        {
            foreach (var commandType in commandTypes)
            {
                if (!typeof(ICommand).GetTypeInfo().IsAssignableFrom(commandType))
                {
                    throw new ArgumentException($"Type {commandType.PrettyPrint()} is not a {typeof(ICommand).PrettyPrint()}");
                }
                _types.Add(commandType);
            }
            return this;
        }

        public IDomainContainer AddEvents(IEnumerable<Type> aggregateEventTypes)
        {
            foreach (var aggregateEventType in aggregateEventTypes)
            {
                if (!typeof(IAggregateEvent).GetTypeInfo().IsAssignableFrom(aggregateEventType))
                {
                    throw new ArgumentException($"Type {aggregateEventType.PrettyPrint()} is not a {typeof(IAggregateEvent).PrettyPrint()}");
                }
                _types.Add(aggregateEventType);
            }
            return this;
        }

        public IDomainContainer AddJobs(IEnumerable<Type> jobTypes)
        {
            foreach (var jobType in jobTypes)
            {
                if (!typeof(IJob).GetTypeInfo().IsAssignableFrom(jobType))
                {
                    throw new ArgumentException($"Type {jobType.PrettyPrint()} is not a {typeof(IJob).PrettyPrint()}");
                }
                _types.Add(jobType);
            }
            return this;
        }

        public IDomainContainer AddRules(IEnumerable<Type> ruleTypes)
        {
            foreach (var ruleType in ruleTypes)
            {
                if (!typeof(IRule).GetTypeInfo().IsAssignableFrom(ruleType))
                {
                    throw new ArgumentException($"Type {ruleType.PrettyPrint()} is not a {typeof(IRule<>).PrettyPrint()}");
                }
                _types.Add(ruleType);
            }
            return this;
        }

        protected override void RegisterDefaults(IServiceCollection serviceCollection)
        {
            base.RegisterDefaults(serviceCollection);

            serviceCollection.TryAddTransient<ICommandBus, CommandBus>();
            serviceCollection.TryAddTransient<IQueryProcessor, QueryProcessor>();
            serviceCollection.TryAddTransient<IAggregateStore, AggregateStore>();
            serviceCollection.TryAddTransient<IAggregateFactory, AggregateFactory>();
            serviceCollection.TryAddTransient<ISerializedCommandPublisher, SerializedCommandPublisher>();
            
            serviceCollection.TryAddSingleton<ICommandDefinitionService, CommandDefinitionService>();
            serviceCollection.TryAddSingleton<IEventDefinitionService, EventDefinitionService>();
            serviceCollection.TryAddSingleton<IJobDefinitionService, JobDefinitionService>();
            serviceCollection.TryAddSingleton<IRuleDefinitionService, RuleDefinitionService>();

            serviceCollection.TryAddTransient<IEventJsonSerializer, EventJsonSerializer>();
            serviceCollection.TryAddTransient<IJobScheduler, InstantJobScheduler>();
            serviceCollection.TryAddTransient<IJobRunner, JobRunner>();

            serviceCollection.TryAddTransient<IDomainEventPublisher, DomainEventPublisher>();
            serviceCollection.TryAddTransient<ISerializedCommandPublisher, SerializedCommandPublisher>();
            serviceCollection.TryAddTransient<IDispatchToEventSubscribers, DispatchToEventSubscribers>();
            serviceCollection.TryAddSingleton<IDomainEventFactory, DomainEventFactory>();
            serviceCollection.TryAddTransient<IValidator, Validator>();

        }
    }
}

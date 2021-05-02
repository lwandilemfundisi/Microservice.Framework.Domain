using Microservice.Framework.Domain.Events;
using Microservice.Framework.Common;
using Microservice.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace Microservice.Framework.Domain.Subscribers
{
    public class DispatchToEventSubscribers : IDispatchToEventSubscribers
    {
        private static readonly Type SubscribeSynchronousToType = typeof(ISubscribeSynchronousTo<,,>);
        private static readonly Type SubscribeAsynchronousToType = typeof(ISubscribeAsynchronousTo<,,>);

        private readonly ILogger<DispatchToEventSubscribers> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISetup _setup;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _memoryCache;

        private class SubscriberInfomation
        {
            public Type SubscriberType { get; set; }
            public Func<object, IDomainEvent, CancellationToken, Task> HandleMethod { get; set; }
        }

        public DispatchToEventSubscribers(
            ILogger<DispatchToEventSubscribers> logger,
            IServiceProvider serviceProvider,
            ISetup setup,
            Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _setup = setup;
            _memoryCache = memoryCache;
        }

        public async Task DispatchToSynchronousSubscribersAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
        {
            foreach (var domainEvent in domainEvents)
            {
                await DispatchToSubscribersAsync(
                        domainEvent,
                        SubscribeSynchronousToType,
                        !_setup.ThrowSubscriberExceptions,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public Task DispatchToAsynchronousSubscribersAsync(
            IDomainEvent domainEvent,
            CancellationToken cancellationToken)
        {
            return DispatchToSubscribersAsync(domainEvent, SubscribeAsynchronousToType, true, cancellationToken);
        }

        private async Task DispatchToSubscribersAsync(
            IDomainEvent domainEvent,
            Type subscriberType,
            bool swallowException,
            CancellationToken cancellationToken)
        {
            var subscriberInfomation = await GetSubscriberInfomationAsync(
                    domainEvent.GetType(),
                    subscriberType,
                    cancellationToken)
                .ConfigureAwait(false);
            var subscribers = _serviceProvider.GetServices(subscriberInfomation.SubscriberType).ToList();

            if (!subscribers.Any())
            {
                _logger.LogDebug($"Didn't find any subscribers to '{domainEvent.EventType.PrettyPrint()}'");
                return;
            }

            foreach (var subscriber in subscribers)
            {
                if(_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
                {
                    _logger.LogTrace($"Calling HandleAsync on handler '{subscriber.GetType().PrettyPrint()}' " +
                                   $"for aggregate event '{domainEvent.EventType.PrettyPrint()}'");
                }

                try
                {
                    await subscriberInfomation.HandleMethod(subscriber, domainEvent, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e) when (swallowException)
                {
                    _logger.LogError(e, $"Subscriber '{subscriberInfomation.SubscriberType.PrettyPrint()}' threw " +
                                  $"'{e.GetType().PrettyPrint()}' while handling '{domainEvent.EventType.PrettyPrint()}': {e.Message}");
                }
            }
        }

        private Task<SubscriberInfomation> GetSubscriberInfomationAsync(
            Type domainEventType,
            Type subscriberType,
            CancellationToken cancellationToken)
        {
            return _memoryCache.GetOrCreate(
                CacheKey.With(GetType(), domainEventType.GetCacheKey(), subscriberType.GetCacheKey()),
                e =>
                {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                    var arguments = domainEventType
                        .GetTypeInfo()
                        .GetInterfaces()
                        .Single(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEvent<,,>))
                        .GetTypeInfo()
                        .GetGenericArguments();

                    var handlerType = subscriberType.MakeGenericType(arguments[0], arguments[1], arguments[2]);
                    var invokeHandleAsync = ReflectionHelper.CompileMethodInvocation<Func<object, IDomainEvent, CancellationToken, Task>>(handlerType, "HandleAsync");

                    return Task.FromResult(new SubscriberInfomation
                    {
                        SubscriberType = handlerType,
                        HandleMethod = invokeHandleAsync,
                    });
                });
        }
    }
}
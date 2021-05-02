using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microservice.Framework.Common;
using Microservice.Framework.Ioc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Queries
{
    public class QueryProcessor : IQueryProcessor
    {
        private class CacheItem
        {
            public Type QueryHandlerType { get; set; }
            public Func<IQueryHandler, IQuery, CancellationToken, Task> HandlerFunc { get; set; }
        }

        private readonly ILogger<QueryProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _memoryCache;

        public QueryProcessor(
            ILogger<QueryProcessor> logger,
            IServiceProvider serviceProvider,
            Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        public async Task<TResult> ProcessAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken)
        {
            var queryType = query.GetType();
            var cacheItem = await GetCacheItemAsync(queryType, cancellationToken).ConfigureAwait(false);

            var queryHandler = (IQueryHandler)_serviceProvider.GetService(cacheItem.QueryHandlerType);

            if(_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
            {
                _logger.LogTrace($"Executing query '{queryType.PrettyPrint()}' ({cacheItem.QueryHandlerType.PrettyPrint()}) by using query handler '{queryHandler.GetType().PrettyPrint()}'");
            }

            var task = (Task<TResult>) cacheItem.HandlerFunc(queryHandler, query, cancellationToken);

            return await task.ConfigureAwait(false);
        }

        public TResult Process<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken)
        {
            var result = default(TResult);
            using (var a = AsyncHelper.Wait)
            {
                a.Run(ProcessAsync(query, cancellationToken), r => result = r);
            }
            return result;
        }

        private Task<CacheItem> GetCacheItemAsync(
            Type queryType,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_memoryCache.GetOrCreate(
                CacheKey.With(GetType(), queryType.GetCacheKey()),
                e =>
                {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                    var queryInterfaceType = queryType
                        .GetTypeInfo()
                        .GetInterfaces()
                        .Single(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));
                    var queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, queryInterfaceType.GetTypeInfo().GetGenericArguments()[0]);
                    var invokeExecuteQueryAsync = ReflectionHelper.CompileMethodInvocation<Func<IQueryHandler, IQuery, CancellationToken, Task>>(
                        queryHandlerType,
                        "ExecuteQueryAsync",
                        queryType, typeof(CancellationToken));
                    return new CacheItem
                    {
                        QueryHandlerType = queryHandlerType,
                        HandlerFunc = invokeExecuteQueryAsync
                    };
                }));
        }
    }
}
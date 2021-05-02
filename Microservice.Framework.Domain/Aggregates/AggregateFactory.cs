using Microsoft.Extensions.DependencyInjection;
using Microservice.Framework.Common;
using Microservice.Framework.Ioc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Aggregates
{
    public class AggregateFactory : IAggregateFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly ConcurrentDictionary<Type, AggregateConstruction> AggregateConstructions = 
            new ConcurrentDictionary<Type, AggregateConstruction>();

        public AggregateFactory(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TAggregate> CreateNewAggregateAsync<TAggregate, TIdentity>(TIdentity id)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateConstruction = AggregateConstructions.GetOrAdd(
                typeof(TAggregate),
                _ => CreateAggregateConstruction<TAggregate, TIdentity>());

            var aggregate = aggregateConstruction.CreateInstance(id, _serviceProvider);

            return Task.FromResult((TAggregate)aggregate);
        }

        private static AggregateConstruction CreateAggregateConstruction<TAggregate, TIdentity>()
        {
            var constructorInfos = typeof(TAggregate)
                .GetTypeInfo()
                .GetConstructors()
                .ToList();

            var constructorInfo = constructorInfos.Single(c => c.GetParameters().Any());
            var parameterInfos = constructorInfo.GetParameters();
            var identityType = typeof(TIdentity);

            return new AggregateConstruction(
                parameterInfos,
                constructorInfo,
                identityType);
        }

        private class AggregateConstruction
        {
            private readonly IReadOnlyCollection<ParameterInfo> _parameterInfos;
            private readonly ConstructorInfo _constructorInfo;
            private readonly Type _identityType;

            public AggregateConstruction(
                IReadOnlyCollection<ParameterInfo> parameterInfos,
                ConstructorInfo constructorInfo,
                Type identityType)
            {
                _parameterInfos = parameterInfos;
                _constructorInfo = constructorInfo;
                _identityType = identityType;
            }

            public object CreateInstance(IIdentity identity, IServiceProvider serviceProvider)
            {
                var parameters = new object[_parameterInfos.Count];
                foreach (var parameterInfo in _parameterInfos)
                {
                    if (parameterInfo.ParameterType == _identityType)
                    {
                        parameters[parameterInfo.Position] = identity;
                    }
                    else
                    {
                        parameters[parameterInfo.Position] = serviceProvider.GetRequiredService(parameterInfo.ParameterType);
                    }
                }

                return _constructorInfo.Invoke(parameters);
            }
        }
    }
}

using Microservice.Framework.Common;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Domain.Rules.RuleValidator;
using Microservice.Framework.Domain.Test.Rules;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Test
{
    public class RulesTests
    {
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection().AddLogging();

            _serviceProvider = DomainContainer.New()
                .AddDefaults(typeof(RulesTests).Assembly)
                .ServiceCollection.BuildServiceProvider();
        }

        [Test]
        public async Task RuleTest()
        {
            var validator = _serviceProvider
                .GetService<IValidator>();

            var validationResult = await validator
                .Validate(
                new TestObject() { Name = "Lwandile"},
                null,
                SystemCulture.Default(),
                this.GetType().Assembly,
                CancellationToken.None);

            Assert.IsTrue(validationResult.HasErrors);
        }
    }
}
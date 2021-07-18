using Microservice.Framework.Domain.Rules;
using Microservice.Framework.Domain.Rules.Common;
using Microsoft.Extensions.Logging;

namespace Microservice.Framework.Domain.Test.Rules
{
    public class TestObject
    {
        public string Name { get; set; }

        public int? Age { get; set; }

    }

    public class TestObjectContext
    {

    }

    [Rule(typeof(TestObject))]
    [RuleProperty("Name")]
    [RuleProperty("Age")]
    public class PersonRequiredRule : RequiredRule<TestObject>
    {
        ILogger<PersonRequiredRule> _logger;

        public PersonRequiredRule(ILogger<PersonRequiredRule> logger)
        {
            _logger = logger;
        }
    }
}

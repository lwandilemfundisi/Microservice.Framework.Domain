using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules;
using Microservice.Framework.Domain.Rules.Attributes;
using Microservice.Framework.Domain.Rules.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Test.Rules
{
    public class TestObject
    {
        public string Name { get; set; }

        public int? Age { get; set; }
    }

    [RuleForProperty("Name")]
    public class PersonRequiredRule : RequiredRule<TestObject>
    {
        ILogger<PersonRequiredRule> _logger;

        public PersonRequiredRule(ILogger<PersonRequiredRule> logger)
        {
            _logger = logger;
        }
    }
}

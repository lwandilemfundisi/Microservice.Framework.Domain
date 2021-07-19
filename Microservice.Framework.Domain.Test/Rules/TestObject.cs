using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules;
using Microservice.Framework.Domain.Rules.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using Microsoft.Extensions.Logging;

namespace Microservice.Framework.Domain.Test.Rules
{
    #region Specifications

    public class PersonSpecifications : Specification<TestObject>
    {
        protected override Notification IsNotSatisfiedBecause(TestObject obj)
        {
            if (obj.IsNull())
                return Notification.Create(new Message($"{typeof(TestObject).PrettyPrint()} - is null", SeverityType.Critical));
            if(obj.Name == "Lwandile")
                return Notification.Create(new Message($"{typeof(TestObject).PrettyPrint()} - name is Lwandile", SeverityType.Critical));

            return Notification.CreateEmpty();
        }
    }

    #endregion

    public class TestObject
    {
        public string Name { get; set; }

        public int? Age { get; set; }

        public Specification<TestObject> GetSpecification()
        {
            return new PersonSpecifications();
        }
    }

    public class TestObjectContext
    {

    }

    [Rule(typeof(TestObject))]
    public class PersonRule : Rule<TestObject>
    {
        ILogger<PersonRule> _logger;

        public PersonRule(ILogger<PersonRule> logger)
        {
            _logger = logger;
        }

        protected override Notification OnValidate()
        {
            return Instance.GetSpecification().WhyIsNotSatisfiedBy(Instance);
        }

        protected override bool OnMustValidate()
        {
            return Instance.IsNotNull();
        }
    }
}

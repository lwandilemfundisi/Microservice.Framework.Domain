using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;

namespace Microservice.Framework.Domain.Rules
{
    public interface IRule
    {
       SeverityType Severity { get; set; }

        object Context { get; set; }

        SystemCulture Culture { get; set; }

        object Owner { get; set; }

        string Name { get; set; }

        Notification Validate();

        bool IsPropertyRule { get; set; }

        string PropertyName { get; set; }

        string ReadOnlyMessage { get; set; }

        bool MustValidate();

        string DisplayName { get; set; }

        bool CanExecuteParallel();
    }
}
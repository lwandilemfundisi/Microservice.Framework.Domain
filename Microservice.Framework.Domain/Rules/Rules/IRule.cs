using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules
{
    public interface IRule
    {
       SeverityType Severity { get; set; }

        object Context { get; set; }

        SystemCulture Culture { get; set; }

        object Owner { get; set; }

        string Name { get; set; }

        Task<Notification> Validate(CancellationToken cancellationToken);

        bool IsPropertyRule { get; set; }

        string PropertyName { get; set; }

        string ReadOnlyMessage { get; set; }

        bool MustValidate();

        string DisplayName { get; set; }

        bool CanExecuteParallel();
    }
}
using Microservice.Framework.Domain.Rules.Notifications;
using System.Reflection;

namespace Microservice.Framework.Domain.Rules
{
    public interface IRule
    {
        PropertyInfo Property { get; set; }

        NotificationMessage Validate();

        bool MustValidate();
    }

    public interface IRule<T> : IRule
    {
        T Owner { get; set; }
    }
}

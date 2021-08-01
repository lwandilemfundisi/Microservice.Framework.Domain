using Microservice.Framework.Common;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class InitialsRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (!StringValidationHelper.ValidateString(propertyValue, AllowedCharacter.Alpha, AllowedCharacter.Space, AllowedCharacter.ForwardSlash, AllowedCharacter.Exclamation))
                {
                    notification.AddMessage(CreateMessage("{0} is not valid", DisplayName));
                }

                var terminationCharacter = PropertyValue.AsString().Last().ToString();

                if (!StringValidationHelper.ValidateString(terminationCharacter, AllowedCharacter.Alpha, AllowedCharacter.Space, AllowedCharacter.ForwardSlash, AllowedCharacter.Exclamation))
                {
                    notification.AddMessage(CreateMessage("{0} may not terminate in a {1}", DisplayName, terminationCharacter));
                }
            }

            return Task.FromResult(notification);
        }

        #endregion
    }
}

using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class UppercaseRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            return ValidatePropertyValue(PropertyValue.AsString());
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} cannot have lowercase characters", DisplayName);
        }

        #endregion

        #region Methods

        protected Task<Notification> ValidatePropertyValue(string propertyValue)
        {
            var notification = Notification.CreateEmpty();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (Regex.IsMatch(propertyValue, "(?=.*[a-z])"))
                {
                    notification.AddMessage(OnCreateMessage());
                }                
            }

            return Task.FromResult(notification);
        }

        #endregion
    }
}

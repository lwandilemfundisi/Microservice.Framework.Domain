using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Text.RegularExpressions;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class LowercaseRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Notification OnValidate()
        {
            return ValidatePropertyValue(PropertyValue.AsString());
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} cannot have uppercase characters", DisplayName);
        }

        #endregion

        #region Methods

        protected Notification ValidatePropertyValue(string propertyValue)
        {
            var notification = Notification.CreateEmpty();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (Regex.IsMatch(propertyValue, "(?=.*[A-Z])"))
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return notification;
        }

        #endregion
    }
}

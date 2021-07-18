using Microservice.Framework.Common;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Domain.Rules.Notifications;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class AlphanumericPropertyRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Notification OnValidate()
        {
            return ValidatePropertyValue(PropertyValue.AsString());
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} must be alphanumeric", DisplayName);
        }

        #endregion

        #region Methods

        protected Notification ValidatePropertyValue(string propertyValue)
        {
            var notification = Notification.CreateEmpty();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (!StringValidationHelper.ValidateString(propertyValue, AllowedCharacter.Alpha, AllowedCharacter.Numeric))
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return notification;
        }

        #endregion
    }
}

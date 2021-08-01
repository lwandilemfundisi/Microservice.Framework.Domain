using Microservice.Framework.Common;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class PhoneNumberFormatPropertyRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (IsInternationalFormat)
                {
                    if (!StringValidationHelper.ValidateString(propertyValue, AllowedCharacter.Numeric, AllowedCharacter.Plus, AllowedCharacter.Space))
                    {
                        notification.AddMessage(OnCreateMessage());
                    }
                }
                else
                {
                    if (!StringValidationHelper.ValidateString(propertyValue, AllowedCharacter.Numeric))
                    {
                        notification.AddMessage(OnCreateMessage());
                    }
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual Message OnCreateMessage()
        {
            if (!IsInternationalFormat)
            {
                var propertyValue = PropertyValue.AsString();

                if (propertyValue.Contains(" "))
                {

                    return CreateMessage("{0} may not contain any spaces", DisplayName);
                }

                return CreateMessage("{0} must be numeric", DisplayName);
            }

            return CreateMessage("{0} is invalid", DisplayName);
        }

        #endregion

        #region Properties

        public virtual bool IsInternationalFormat
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}

using Microservice.Framework.Common;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class BasicInputRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (!StringValidationHelper.ValidateString(propertyValue, OnGetAllowedCharacterTypes().ToArray()))
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} must be alphanumeric", DisplayName);
        }

        protected virtual IEnumerable<AllowedCharacter> OnGetAllowedCharacterTypes()
        {
            return new AllowedCharacter[]
            {
                AllowedCharacter.Alpha, 
                AllowedCharacter.Numeric, 
                AllowedCharacter.Space,                    
                AllowedCharacter.Dot, 
                AllowedCharacter.Comma, 
                AllowedCharacter.ForwardSlash
            };
        }

        #endregion
    }
}

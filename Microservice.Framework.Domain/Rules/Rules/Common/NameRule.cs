﻿using Microservice.Framework.Common;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Linq;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class NameRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Notification OnValidate()
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (!StringValidationHelper.ValidateString(propertyValue, AllowedCharacter.Alpha, AllowedCharacter.Space, AllowedCharacter.Apostrophe, AllowedCharacter.Dash, AllowedCharacter.ForwardSlash, AllowedCharacter.Exclamation, AllowedCharacter.Ampersand, AllowedCharacter.RoundBrackets, AllowedCharacter.AccentedVowelsLower))
                {
                    notification.AddMessage(CreateMessage("{0} is not valid", DisplayName));
                    return notification;
                }

                if (StringValidationHelper.ValidateString(propertyValue,  AllowedCharacter.Apostrophe, AllowedCharacter.Dash, AllowedCharacter.ForwardSlash, AllowedCharacter.Exclamation))
                {
                    notification.AddMessage(CreateMessage("{0} must contain at least one letter of the alphabet", DisplayName));
                    return notification;
                }

                var terminationCharacter = PropertyValue.AsString().Last().ToString();

                if (!StringValidationHelper.ValidateString(terminationCharacter, AllowedCharacter.Alpha, AllowedCharacter.Space, AllowedCharacter.RoundBrackets, AllowedCharacter.AccentedVowelsLower))
                {
                    notification.AddMessage(CreateMessage("{0} may not terminate in a {1}", DisplayName, terminationCharacter));
                    return notification;
                }

            }

            return notification;
        }

        #endregion
    }
}

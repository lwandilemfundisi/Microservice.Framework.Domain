using Microservice.Framework.Common;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class EmailPropertyRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            if (propertyValue.IsNotNullOrEmpty())
            {
                var listOfPropertyValues = propertyValue.Split(',');

                List<string> listOfInvalidEmailAddresses = new List<string>();

                for (int j = 0; j < listOfPropertyValues.Length; j++)
			    {
                    string emailAddress = listOfPropertyValues[j];
			        if (!StringValidationHelper.ValidateEmailAddress(emailAddress))
                    {
                        listOfInvalidEmailAddresses.Add(emailAddress);
                    }
			    }
                
                foreach (var invalidEmailAddress in listOfInvalidEmailAddresses)
                {
                    notification.AddMessage(OnCreateMessage(invalidEmailAddress));
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual Message OnCreateMessage(string emailAddress)
        {
            return CreateMessage("'{0}' is an invalid email address", emailAddress);
        }

        #endregion
    }
}

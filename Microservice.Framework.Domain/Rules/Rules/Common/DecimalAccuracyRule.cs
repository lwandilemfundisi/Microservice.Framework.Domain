using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class DecimalAccuracyRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var deimcalValue = OnGetDecimalValue();

            if (deimcalValue.HasValue)
            {
                var accuracy = OnGetAccuracy();
                if (Math.Round(deimcalValue.Value, accuracy) != deimcalValue.Value)
                {
                    notification += OnCreateMessage(accuracy);
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual int OnGetAccuracy()
        {
            return 2;
        }

        protected virtual Message OnCreateMessage(int accuracy)
        {
            return CreateMessage("{0} may only contain {1} decimal place(s)".FormatInvariantCulture(DisplayName, accuracy));
        }

        protected virtual decimal? OnGetDecimalValue()
        {
            return PropertyValue as decimal?;
        }

        #endregion
    }
}

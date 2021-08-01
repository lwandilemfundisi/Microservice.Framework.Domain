using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class DateRangeRule<T> : Rule<T>, IRangeRule where T : class
    {
        #region IRange Members

        public object GetMinimum()
        {
            return OnGetMinimum();
        }

        public object GetMaximum()
        {
            return OnGetMaximum();
        }

        #endregion

        #region Virtual Members

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();
            var propertyValue = PropertyValue as DateTime?;

            if (propertyValue.IsNotNull())
            {
                var minimum = OnGetMinimum();

                var maximum = OnGetMaximum();

                if (propertyValue > maximum || propertyValue < minimum)
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual DateTime OnGetMaximum()
        {
            return DateTime.MaxValue;
        }

        protected virtual DateTime OnGetMinimum()
        {
            return DateTime.MinValue;
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} does not fall within the range of {1} and {2}", DisplayName, OnGetMinimum().ToLongDateString(), OnGetMaximum().ToLongDateString());
        }

        #endregion
    }
}

using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class BytesRangeRule<T> : Rule<T>, IRangeRule where T : class
    {
        #region IRange Members

        public object GetMaximum()
        {
            return OnGetMaximum();
        }

        public object GetMinimum()
        {
            return OnGetMinimum();
        }

        #endregion

        #region Virtual Methods

        protected abstract int OnGetMaximum();

        protected virtual int OnGetMinimum()
        {
            return 1;
        }

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue as byte[];

            if (propertyValue.IsNotNull())
            {
                var minimum = OnGetMinimum();
                var maximum = OnGetMaximum();

                if (propertyValue.Length < minimum || propertyValue.Length > maximum)
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual Message OnCreateMessage()
        {
            return OnCreateMessage(DisplayName, OnGetMinimum(), OnGetMaximum());
        }

        protected virtual Message OnCreateMessage(string displayName, int minimum, int maximum)
        {
            return CreateMessage("{0} does not fall between the range of {1} and {2} bytes", DisplayName, minimum, maximum);
        }

        #endregion
    }

    public abstract class BytesRangeRule<T, C> : BytesRangeRule<T>
        where T : class
        where C : class
    {
        #region Methods

        public C GetContext()
        {
            return (C)Context;
        }

        #endregion
    }
}

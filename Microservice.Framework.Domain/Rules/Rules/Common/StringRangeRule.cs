using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class StringRangeRule<T> : Rule<T>, IRangeRule where T : class
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

        protected virtual int OnGetMaximum()
        {
            return 255;
        }

        protected virtual int OnGetMinimum()
        {
            return 1;
        }

        protected override Notification OnValidate()
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            //rule should not check empty values. Required Rules must validate empty or null values
            if (propertyValue.IsNotNullOrEmpty())
            {
                var minimum = OnGetMinimum();
                var maximum = OnGetMaximum();

                if (propertyValue.Length < minimum || propertyValue.Length > maximum)
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return notification;
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} does not fall between the range of {1} and {2}", DisplayName, OnGetMinimum(), OnGetMaximum());
        }

        #endregion
    }

    public abstract class StringRangeRule<T, C> : StringRangeRule<T> 
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

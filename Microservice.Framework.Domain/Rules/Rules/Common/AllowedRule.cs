using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections;
using System.Linq;
using Microservice.Framework.Common;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class AllowedRule<T> : Rule<T>, IAllowedRule where T : class
    {
        #region IAllowedRule Members

        public IEnumerable GetAllowedValues()
        {
            return OnGetAllowedValues();
        }

        #endregion

        #region Virtual Methods

        protected override Notification OnValidate()
        {
            var notification = Notification.CreateEmpty();

            if (PropertyHasValue())
            {
                if (!OnContainsPropertyValue(GetAllowedValues(), PropertyValue))
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }
            
            return notification;
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} is not allowed", DisplayName);
        }

        protected abstract IEnumerable OnGetAllowedValues();

        protected virtual bool OnContainsPropertyValue(IEnumerable allowedValues, object propertyValue)
        {
            return allowedValues.OfType<object>().Contains(v => v.Equals(propertyValue));
        }

        #endregion
    }

    public abstract class AllowedRule<T, C> : AllowedRule<T> 
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

using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;

namespace Microservice.Framework.Domain.Rules.Common
{ 
    public abstract class ApplicableRule<T> : Rule<T>, IApplicableRule where T : class
    {
        private bool? isApplicable;

        #region IApplicableRule Members

        public bool IsApplicable()
        {
            if (isApplicable.IsNull())
            {
                isApplicable = OnIsApplicable();
            }

            return isApplicable.Value;
        }

        #endregion

        #region Virtual Methods

        protected override Notification OnValidate()
        {
            var notification = Notification.CreateEmpty();

            if (!IsApplicable())
            {
                if (IsPropertyRule)
                {
                    if (PropertyHasValue())
                    {
                        notification += OnCreateApplicableMessage();
                    }
                }
                else
                {
                    notification += OnCreateApplicableMessage();
                }
            }

            return notification;
        }

        protected abstract bool OnIsApplicable();

        protected virtual Message OnCreateApplicableMessage()
        {
            return CreateMessage("{0} is not applicable".FormatInvariantCulture(DisplayName));
        }

        protected override Message CreateMessage(string message, params object[] values)
        {
            var result = base.CreateMessage(message, values);

            result.MessageType = MessageType.NotApplicable;

            return result;
        }

        #endregion
    }

    public abstract class ApplicableRule<T, C> : ApplicableRule<T>
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

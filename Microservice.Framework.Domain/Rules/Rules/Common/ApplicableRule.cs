using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{ 
    public abstract class ApplicableRule<T> 
        : Rule<T>, IApplicableRule where T : class
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

        protected override async Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            if (!IsApplicable())
            {
                if (IsPropertyRule)
                {
                    if (PropertyHasValue())
                    {
                        notification += await OnCreateApplicableMessage();
                    }
                }
                else
                {
                    notification += await OnCreateApplicableMessage();
                }
            }

            return notification;
        }

        protected abstract bool OnIsApplicable();

        protected virtual Task<Message> OnCreateApplicableMessage()
        {
            return Task.FromResult(CreateMessage("{0} is not applicable".FormatInvariantCulture(DisplayName)));
        }

        protected override Message CreateMessage(string message, params object[] values)
        {
            var result = base.CreateMessage(message, values);

            result.MessageType = MessageType.NotApplicable;

            return result;
        }

        #endregion
    }

    public abstract class ApplicableRule<T, C> 
        : ApplicableRule<T>
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

using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class CollectionRangeRule<T> : Rule<T>, IRangeRule where T : class
    {
        #region IRangeRule Members

        public object GetMinimum()
        {
            return OnGetMinimum();
        }

        public object GetMaximum()
        {
            return OnGetMaximum();
        }

        #endregion

        #region Properties

        protected abstract int CollectionLength { get; }

        #endregion

        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();
            var minimum = GetMinimum() as int?;
            var maximum = GetMaximum() as int?;
            var collectionLength = CollectionLength;

            if (minimum.HasValue 
             && minimum.Value > 0 
             && PropertyValue.IsNull())
            {
                notification += OnCreateMessage(minimum, maximum);
            }
            else if (minimum.HasValue
                  && collectionLength < minimum.Value)
            {
                notification += OnCreateMessage(minimum, maximum);
            }
            else if (maximum.HasValue
                  && collectionLength > maximum.Value)
            {
                notification += OnCreateMessage(minimum, maximum);
            }

            return Task.FromResult(notification);
        }

        protected virtual int? OnGetMinimum()
        {
            return null;
        }

        protected virtual int? OnGetMaximum()
        {
            return null;
        }

        protected virtual Message OnCreateMessage(int? minimum, int? maximum)
        {
            if (minimum.GetValueOrDefault() == 0)
            {
                return CreateMessage("The {0} list may not be more than {1}", DisplayName, maximum);
            }
            else if (maximum.HasValue)
            {
                return CreateMessage("The {0} list must be between {1} and {2}", DisplayName, minimum, maximum);
            }
            else
            {
                if (minimum.GetValueOrDefault() == 1)
                {
                    return CreateMessage("Please specify at least one {0} item", DisplayName);
                }
                return CreateMessage("The {0} list may not be less than {1}", DisplayName, minimum);
            }
        }

        #endregion
    }
}

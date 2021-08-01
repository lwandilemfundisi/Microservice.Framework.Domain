using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public class DecimalRangeRule<T> : Rule<T>, IRangeRule where T : class
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

        #region Properties

        protected virtual bool IsCurrency
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Virtual Members

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue as decimal?;

            if (propertyValue.IsNotNull())
            {
                var minimum = GetMinimum() as decimal?;
                var maximum = GetMaximum() as decimal?;

                if (propertyValue < minimum.GetValueOrDefault(decimal.MinValue) || propertyValue > maximum.GetValueOrDefault(decimal.MaxValue))
                {
                    if (minimum.HasValue && maximum.HasValue)
                    {
                        notification.AddMessage(OnCreateMessage(minimum.Value, maximum.Value));
                    }
                    else if (minimum.HasValue)
                    {
                        notification.AddMessage(OnCreateMinimumMessage(minimum.Value));
                    }
                    else if (maximum.HasValue)
                    {
                        notification.AddMessage(OnCreateMaximumMessage(maximum.Value));
                    }
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual decimal? OnGetMinimum()
        {
            return null;
        }

        protected virtual decimal? OnGetMaximum()
        {
            return null;
        }

        protected virtual Message OnCreateMessage(decimal minimum, decimal maximum)
        {
            if (IsCurrency)
            {

                return CreateMessage("{0} does not fall within the range of {1} and {2}", DisplayName, minimum.FormatCurrencyValueNoSpace(Culture.CurrencySymbol), maximum.FormatCurrencyValueNoSpace(Culture.CurrencySymbol));
            }

            return CreateMessage("{0} does not fall within the range of {1:#0.00} and {2:#0.00}", DisplayName, minimum, maximum);
        }

        protected virtual Message OnCreateMinimumMessage(decimal minimum)
        {
            if (IsCurrency)
            {
                return CreateMessage("{0} may not be less than {1}", DisplayName, minimum.FormatCurrencyValueNoSpace());
            }
            return CreateMessage("{0} may not be less than {1}", DisplayName, minimum);
        }

        protected virtual Message OnCreateMaximumMessage(decimal maximum)
        {
            if (IsCurrency)
            {
                return CreateMessage("{0} may not be greater than {1}", DisplayName, maximum.FormatCurrencyValueNoSpace());
            }
            return CreateMessage("{0} may not be greater than {1}", DisplayName, maximum);
        }

        #endregion
    }

    public class DecimalRangeRule<T, C, Z> : DecimalRangeRule<T>
        where T : class
        where C : class
        where Z : SystemCulture
    {
        #region Methods

        public C GetContext()
        {
            return (C)Context;
        }

        public Z GetCulture()
        {
            return (Z)Culture;
        }
        #endregion
    }
}

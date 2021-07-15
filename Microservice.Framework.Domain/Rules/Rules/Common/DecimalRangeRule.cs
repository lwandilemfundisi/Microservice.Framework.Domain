using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class DecimalRangeRule<T> : Rule<T>, IRangeRule<T> where T : class
    {
        #region Properties

        protected virtual bool IsCurrency
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Virtual Methods

        protected override string ValidationMessage
        {
            get
            {
                var message = string.Empty;

                if (GetMinimum().IsNotNull() && GetMaximum().IsNotNull())
                {
                    message = "{0} does not fall within the range of " + $"{Format(OnGetMinimum())} and {Format(OnGetMaximum())}";
                }
                else if (GetMinimum().IsNotNull())
                {
                    message = "{0} may not be less than " + $"{Format(OnGetMinimum())}";
                }
                else if (GetMaximum().IsNotNull())
                {
                    message = "{0} may not be more than " + $"{Format(OnGetMaximum())}";
                }

                return message;
            }
        }

        protected override bool ValidationCondition()
        {
            var propertyValue = PropertyValue as decimal?;

            if (propertyValue.IsNotNull())
            {
                var minimum = GetMinimum() as decimal?;
                var maximum = GetMaximum() as decimal?;

                if (propertyValue < minimum.GetValueOrDefault(decimal.MinValue) || propertyValue > maximum.GetValueOrDefault(decimal.MaxValue))
                {
                    if (minimum.HasValue && maximum.HasValue)
                    {
                        return false;
                    }
                    else if (minimum.HasValue)
                    {
                        return false;
                    }
                    else if (maximum.HasValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected virtual decimal? OnGetMinimum()
        {
            return null;
        }

        protected virtual decimal? OnGetMaximum()
        {
            return null;
        }

        #endregion

        #region IRangeRule Members

        public object GetMaximum()
        {
            return OnGetMaximum();
        }

        public object GetMinimum()
        {
            return OnGetMinimum();
        }

        #endregion

        #region Private Methods

        private string Format(decimal? value)
        {
            if(IsCurrency)
            {
                return value.FormatCurrencyValueNoSpace(Culture.CurrencySymbol);
            }

            return "{0:#0.00}".FormatInvariantCulture(value);
        }

        #endregion
    }
}

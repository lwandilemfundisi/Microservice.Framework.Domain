using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class DateRangeRule<T> : Rule<T>, IRangeRule<T> where T : class
    {
        #region Virtual Methods

        protected override string ValidationMessage => "{0} does not fall within the range of " + $"{OnGetMinimum().ToLongDateString()} and {OnGetMaximum().ToLongDateString()}";

        protected override bool ValidationCondition()
        {
            var propertyValue = PropertyValue as DateTime?;

            if (propertyValue.IsNotNull())
            {
                var minimum = OnGetMinimum();

                var maximum = OnGetMaximum();

                if (propertyValue > maximum || propertyValue < minimum)
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual DateTime OnGetMaximum()
        {
            return DateTime.MaxValue;
        }

        protected virtual DateTime OnGetMinimum()
        {
            return DateTime.MinValue;
        }

        #endregion

        #region IRangeRule Members

        public object GetMaximum()
        {
            return OnGetMaximum();
        }

        public object GetMinimum()
        {
            return OnGetMaximum();
        }

        #endregion
    }
}

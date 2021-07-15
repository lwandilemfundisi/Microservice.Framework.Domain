using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class LongRangeRule<T> : Rule<T>, IRangeRule<T> where T : class
    {
        #region Virtual Methods

        protected override string ValidationMessage => "{0} does not fall within the range of " + $"{OnGetMinimum()} and {OnGetMaximum()}";

        protected override bool ValidationCondition()
        {
            var propertyValue = PropertyValue as long?;

            if (propertyValue.IsNotNull())
            {
                var minimum = GetMinimum() as long?;
                var maximum = GetMaximum() as long?;

                if (propertyValue < minimum || propertyValue > maximum)
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual long OnGetMinimum()
        {
            return long.MinValue;
        }

        protected virtual long OnGetMaximum()
        {
            return long.MaxValue;
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
    }
}

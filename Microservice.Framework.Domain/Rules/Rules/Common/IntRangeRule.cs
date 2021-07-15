using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class IntRangeRule<T> : Rule<T>, IRangeRule<T> where T : class
    {
        #region Virtual Methods

        protected override string ValidationMessage => "{0} does not fall within the range of " + $"{OnGetMinimum()} and {OnGetMaximum()}";

        protected override bool ValidationCondition()
        {
            var propertyValue = PropertyValue as int?;

            if (propertyValue.IsNotNull())
            {
                var minimum = GetMinimum() as int?;
                var maximum = GetMaximum() as int?;

                if (propertyValue < minimum || propertyValue > maximum)
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual int OnGetMinimum()
        {
            return int.MinValue;
        }

        protected virtual int OnGetMaximum()
        {
            return int.MaxValue;
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

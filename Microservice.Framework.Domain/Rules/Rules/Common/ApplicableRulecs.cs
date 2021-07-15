using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class ApplicableRulecs<T> : Rule<T>, IApplicableRule<T> where T : class
    {
        private bool? isApplicable;

        #region Virtual Methods

        protected override string ValidationMessage => "{0} is not applicable";

        protected override bool ValidationCondition()
        {
            if (!IsApplicable())
            {
                if (PropertyHasValue())
                {
                    return false;
                }
            }

            return true;
        }

        protected abstract bool OnIsApplicable();

        #endregion

        #region IApplicableRule

        public bool IsApplicable()
        {
            if (isApplicable.IsNull())
            {
                isApplicable = OnIsApplicable();
            }

            return isApplicable.Value;
        }

        #endregion
    }
}

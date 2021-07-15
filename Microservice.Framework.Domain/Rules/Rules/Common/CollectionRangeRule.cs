using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class CollectionRangeRule<T> : Rule<T>, IRangeRule<T> where T : class
    {
        #region Virtual Methods

        protected abstract int CollectionLength { get; }

        protected override string ValidationMessage
        {
            get
            {
                if (OnGetMinimum().GetValueOrDefault() == 0)
                {
                    return "The {0} list may not be more than " + $"{OnGetMaximum()}";
                }
                else if (OnGetMaximum().HasValue)
                {
                    return "The {0} list must be between " + $"{OnGetMinimum()} and {OnGetMaximum()}";
                }
                else
                {
                    if (OnGetMinimum().GetValueOrDefault() == 1)
                    {
                        return "Please specify at least one {0} item";
                    }
                    return "The {0} list may not be less than " + $"{OnGetMinimum()}";
                }
            }
        }

        protected override bool ValidationCondition()
        {
            var minimum = GetMinimum() as int?;
            var maximum = GetMaximum() as int?;
            var collectionLength = CollectionLength;

            if (minimum.HasValue
             && minimum.Value > 0
             && PropertyValue.IsNull())
            {
                return false;
            }
            else if (minimum.HasValue
                  && collectionLength < minimum.Value)
            {
                return false;
            }
            else if (maximum.HasValue
                  && collectionLength > maximum.Value)
            {
                return false;
            }

            return true;
        }

        protected virtual int? OnGetMinimum()
        {
            return null;
        }

        protected virtual int? OnGetMaximum()
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
    }
}

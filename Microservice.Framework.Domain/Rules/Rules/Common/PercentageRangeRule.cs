namespace Microservice.Framework.Domain.Rules.Common
{
    public class PercentageRangeRule<T> : DecimalRangeRule<T>, IRangeRule where T : class
    {
        #region Virtual Members

        protected override decimal? OnGetMinimum()
        {
            return 0.0m;
        }

        protected override decimal? OnGetMaximum()
        {
            return 100.0m;
        }

        #endregion
    }

    public class PercentageRangeRule<T, C> : PercentageRangeRule<T>
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

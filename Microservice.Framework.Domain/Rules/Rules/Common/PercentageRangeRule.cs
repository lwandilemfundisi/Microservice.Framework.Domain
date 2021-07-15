using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class PercentageRangeRule<T> : DecimalRangeRule<T> where T : class
    {
    }
}

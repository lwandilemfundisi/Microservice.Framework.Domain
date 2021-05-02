using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static bool IsAfter(this DateTimeOffset me, DateTimeOffset before)
        {
            return me > before;
        }

        public static bool IsBefore(this DateTimeOffset me, DateTimeOffset after)
        {
            return me < after;
        }
    }
}

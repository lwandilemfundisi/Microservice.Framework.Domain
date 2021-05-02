using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Aggregates
{
    public class AggregateName : SingleValueObject<string>, IAggregateName
    {
        public AggregateName(string value)
            : base(value)
        {

        }
    }
}

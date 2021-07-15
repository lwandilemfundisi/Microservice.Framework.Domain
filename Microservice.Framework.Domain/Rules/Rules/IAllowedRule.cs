using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Microservice.Framework.Domain.Rules
{
    public interface IAllowedRule<T> : IRule<T>
    {
        IEnumerable GetAllowedValues();
    }
}

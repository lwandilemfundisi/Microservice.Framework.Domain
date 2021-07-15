using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microservice.Framework.Domain.Rules
{
    public interface IApplicableRule<T> : IRule<T>
    {
        bool IsApplicable();
    }
}

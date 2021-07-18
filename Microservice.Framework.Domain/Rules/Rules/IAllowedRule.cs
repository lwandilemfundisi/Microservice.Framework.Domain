using System.Collections;

namespace Microservice.Framework.Domain.Rules
{
    public interface IAllowedRule : IRule
    {
        IEnumerable GetAllowedValues();
    }
}

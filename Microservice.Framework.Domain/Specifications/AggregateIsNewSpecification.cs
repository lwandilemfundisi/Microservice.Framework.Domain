using Microservice.Framework.Domain.Aggregates;
using System.Collections.Generic;

namespace Microservice.Framework.Domain
{
    public class AggregateIsNewSpecification : Specification<IAggregateRoot>
    {
        protected override IEnumerable<string> IsNotSatisfiedBecause(IAggregateRoot obj)
        {
            if (!obj.IsNew)
            {
                yield return $"'{obj.Name}' with ID '{obj.GetIdentity()}' is not new";
            }
        }
    }
}
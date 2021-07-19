using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Collections.Generic;

namespace Microservice.Framework.Domain
{
    public class AggregateIsNewSpecification : Specification<IAggregateRoot>
    {
        protected override Notification IsNotSatisfiedBecause(IAggregateRoot obj)
        {
            if (!obj.IsNew)
            {
                return Notification
                    .Create(
                    new Message($"'{obj.Name}' with ID '{obj.GetIdentity()}' is not new", SeverityType.Critical));
            }

            return Notification.CreateEmpty();
        }
    }
}
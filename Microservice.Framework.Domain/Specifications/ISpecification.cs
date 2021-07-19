using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T obj);

        Notification WhyIsNotSatisfiedBy(T obj);
    }
}

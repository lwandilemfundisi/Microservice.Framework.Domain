using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microservice.Framework.Domain
{
    public abstract class Specification<T> : ISpecification<T>
    {
        public bool IsSatisfiedBy(T obj)
        {
            return !IsNotSatisfiedBecause(obj).Any();
        }

        public Notification WhyIsNotSatisfiedBy(T obj)
        {
            return IsNotSatisfiedBecause(obj);
        }

        protected abstract Notification IsNotSatisfiedBecause(T obj);
    }
}

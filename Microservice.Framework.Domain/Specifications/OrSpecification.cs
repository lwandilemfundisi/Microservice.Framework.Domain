using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microservice.Framework.Domain
{
    public class OrSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _specification1;
        private readonly ISpecification<T> _specification2;

        public OrSpecification(
            ISpecification<T> specification1,
            ISpecification<T> specification2)
        {
            _specification1 = specification1 ?? throw new ArgumentNullException(nameof(specification1));
            _specification2 = specification2 ?? throw new ArgumentNullException(nameof(specification2));
        }

        protected override Notification IsNotSatisfiedBecause(T obj)
        {
            var reasons1 = _specification1.WhyIsNotSatisfiedBy(obj);
            var reasons2 = _specification2.WhyIsNotSatisfiedBy(obj);

            if (!reasons1.HasErrors || !reasons2.HasErrors)
            {
                return Notification.CreateEmpty();
            }

            return Notification.Add(reasons1, reasons2);
        }
    }
}
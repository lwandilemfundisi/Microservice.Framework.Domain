using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microservice.Framework.Domain
{
    public class AllSpecifications<T> : Specification<T>
    {
        private readonly IReadOnlyList<ISpecification<T>> _specifications;

        public AllSpecifications(
            IEnumerable<ISpecification<T>> specifications)
        {
            var specificationList = (specifications ?? Enumerable.Empty<ISpecification<T>>()).ToList();

            if (!specificationList.Any()) throw new ArgumentException("Please provide some specifications", nameof(specifications));

            _specifications = specificationList;
        }

        protected override Notification IsNotSatisfiedBecause(T obj)
        {
            return Notification
                .Create(
                _specifications.SelectMany(s => s.WhyIsNotSatisfiedBy(obj)).ToList());
        }
    }
}
using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microservice.Framework.Domain
{
    public class AtLeastSpecification<T> : Specification<T>
    {
        private readonly int _requiredSpecifications;
        private readonly IReadOnlyList<ISpecification<T>> _specifications;

        public AtLeastSpecification(
            int requiredSpecifications,
            IEnumerable<ISpecification<T>> specifications)
        {
            var specificationList = (specifications ?? Enumerable.Empty<ISpecification<T>>()).ToList();

            if (requiredSpecifications <= 0)
                throw new ArgumentOutOfRangeException(nameof(requiredSpecifications));
            if (!specificationList.Any())
                throw new ArgumentException("Please provide some specifications", nameof(specifications));
            if (requiredSpecifications > specificationList.Count)
                throw new ArgumentOutOfRangeException($"You required '{requiredSpecifications}' to be met, but only '{specificationList.Count}' was supplied");

            _requiredSpecifications = requiredSpecifications;
            _specifications = specificationList;
        }

        protected override Notification IsNotSatisfiedBecause(T obj)
        {
            var notStatisfiedReasons = _specifications
                .Select(s => new
                    {
                        Specification = s,
                        WhyIsNotStatisfied = s.WhyIsNotSatisfiedBy(obj)
                    })
                .Where(a => a.WhyIsNotStatisfied.HasErrors)
                .Select(a => $"{a.Specification.GetType().PrettyPrint()}: {a.WhyIsNotStatisfied}");

            return (_specifications.Count - notStatisfiedReasons.Count()) >= _requiredSpecifications
                ? Notification.CreateEmpty()
                : Notification
                    .Create(notStatisfiedReasons.Select(r => new Message(r)).ToList());
        }
    }
}
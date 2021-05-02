using Microservice.Framework.Domain.Jobs;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerJobExtensions
    {
        public static IDomainContainer AddJobs(
            this IDomainContainer domainContainer,
            params Type[] jobTypes)
        {
            return domainContainer.AddJobs(jobTypes);
        }

        public static IDomainContainer AddJobs(
            this IDomainContainer domainContainer,
            Assembly fromAssembly,
            Predicate<Type> predicate = null)
        {
            predicate = predicate ?? (t => true);
            var jobTypes = fromAssembly
                .GetTypes()
                .Where(type => !type.GetTypeInfo().IsAbstract && type.IsAssignableTo<IJob>())
                .Where(t => !t.HasConstructorParameterOfType(i => i.IsAssignableTo<IJob>()))
                .Where(t => predicate(t));
            return domainContainer.AddJobs(jobTypes);
        }
    }
}

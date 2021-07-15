using Microservice.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain
{
    public interface IDomainContainer : IContainer
    {
        IDomainContainer AddCommands(IEnumerable<Type> commandTypes);

        IDomainContainer AddJobs(IEnumerable<Type> jobTypes);

        IDomainContainer AddEvents(IEnumerable<Type> aggregateEventTypes);

        IDomainContainer AddRules(IEnumerable<Type> ruleTypes);
    }
}

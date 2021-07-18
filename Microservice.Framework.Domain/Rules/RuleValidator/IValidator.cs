using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.RuleValidator
{
    public interface IValidator
    {
        Task<Notification> Validate(
            IEnumerable<IRule> rules, 
            CancellationToken cancellationToken);

        Task<Notification> Validate(
            object instance, 
            object context, 
            SystemCulture culture, 
            Assembly assembly, 
            CancellationToken cancellationToken);

        Task<Notification> Validate(
            object instance, 
            object context, 
            SystemCulture culture, 
            IEnumerable<Assembly> assemblies, 
            CancellationToken cancellationToken);

        Task<IEnumerable<IRule>> CreateRules(
            object context, 
            SystemCulture culture, 
            object owner, 
            Type ownerType, 
            RuleTypeEntry ruleTypeEntry, 
            CancellationToken cancellationToken);

        Task<IEnumerable<IRule>> ResolveRules(
            object instance, 
            object context, 
            SystemCulture culture, 
            Assembly assembly, 
            CancellationToken cancellationToken);

        Task<IEnumerable<IRule>> ResolveRules(
            object instance, 
            object context, 
            SystemCulture culture, 
            CancellationToken cancellationToken);
    }
}

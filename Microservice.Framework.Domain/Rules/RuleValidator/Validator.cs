using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.RuleValidator
{
    public class Validator : IValidator
    {
        private static object resolveRulesLock = new object();
        private static object resolveEntityRulesLock = new object();
        private readonly IServiceProvider _serviceProvider;

        private static IDictionary<Assembly, IList<RuleTypeEntry>> assemblyRules = new Dictionary<Assembly, IList<RuleTypeEntry>>();
        private static Dictionary<Type, Dictionary<Assembly, IList<RuleTypeEntry>>> entityRules = new Dictionary<Type, Dictionary<Assembly, IList<RuleTypeEntry>>>();

        #region Constructors

        public Validator(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Methods

        public async Task<Notification> Validate(
            IEnumerable<IRule> rules, 
            CancellationToken cancellationToken)
        {
            var applicableRules = rules.OfType<IApplicableRule>();
            var requiredRules = rules.OfType<IRequiredRule>();
            var validationRules = rules.Except(applicableRules.Cast<IRule>()).Except(requiredRules.Cast<IRule>());

            var failedRequiredRules = new List<IRequiredRule>();
            var failedApplicableRules = new List<IApplicableRule>();

            var notification = Notification.CreateEmpty();

            //applicability rules
            foreach (var applicableRule in applicableRules.Where(r => r.MustValidate()))
            {
                var ruleNotification = await applicableRule.Validate(cancellationToken);

                if (ruleNotification.HasErrors)
                {
                    notification += ruleNotification;
                }

                if (!applicableRule.IsApplicable())
                {
                    failedApplicableRules.Add(applicableRule);
                }
            }

            //required rules
            foreach (var requiredRule in requiredRules)
            {
                if (!failedApplicableRules.Contains(r => r.PropertyName == requiredRule.PropertyName && r.Owner == requiredRule.Owner))
                {
                    if (requiredRule.MustValidate())
                    {
                        var ruleNotification = await requiredRule.Validate(cancellationToken);

                        notification += ruleNotification;

                        if (ruleNotification.HasErrors)
                        {
                            failedRequiredRules.Add(requiredRule);
                        }
                    }
                }
            }

            var parallelRules = validationRules.Where(c => c.CanExecuteParallel()).ToList();
            var nonParallelRules = validationRules.Where(c => !c.CanExecuteParallel()).ToList();

            //business and validation rules
            Parallel.ForEach(parallelRules, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async rule =>
            {
                notification += await ValidateRule(rule, failedApplicableRules, failedRequiredRules, cancellationToken);
            });

            foreach (var rule in nonParallelRules)
            {
                notification += await ValidateRule(rule, failedApplicableRules, failedRequiredRules, cancellationToken);
            }
            return notification;
        }

        public async Task<Notification> Validate(
            object instance, 
            object context, 
            SystemCulture culture, 
            Assembly assembly, 
            CancellationToken cancellationToken)
        {
            return await Validate(
                await ResolveRules(
                    instance, 
                    context, 
                    culture, 
                    assembly, 
                    cancellationToken),
                cancellationToken);
        }

        public async Task<Notification> Validate(
            object instance, 
            object context, 
            SystemCulture culture, 
            IEnumerable<Assembly> assemblies, 
            CancellationToken cancellationToken)
        {
            var rules = new List<IRule>();

            foreach (var assembly in assemblies)
            {
                rules.AddRange(
                    await ResolveRules(
                        instance, 
                        context, 
                        culture, 
                        assembly, 
                        cancellationToken));
            }

            return await Validate(
                rules, 
                cancellationToken);
        }

        public Task<IEnumerable<IRule>> CreateRules(
            object context, 
            SystemCulture culture, 
            object owner, 
            Type ownerType, 
            RuleTypeEntry ruleTypeEntry, 
            CancellationToken cancellationToken)
        {
            var validatableType = ownerType;
            var rules = new List<IRule>();
            var ruleAttribute = ruleTypeEntry.RuleAttribute;
            var contextList = ruleTypeEntry.RuleContexts;
            var rulePropertyAttributes = ruleTypeEntry.RuleProperties;
            var ruleType = ruleTypeEntry.RuleType;

            if (ruleAttribute.EntityType.IsNull() ||
                ruleAttribute.EntityType.Equals(validatableType) ||
                validatableType.IsSubclassOf(ruleAttribute.EntityType))
            {
                var contextType = context.GetType();
                var contextEntry = contextList.FirstOrDefault(c => contextType.Equals(c.ContextType) || contextType.IsSubclassOf(c.ContextType));

                if (contextList.Count == 0 || contextEntry.IsNotNull())
                {
                    if (rulePropertyAttributes.Count > 0)
                    {
                        foreach (var rulePropertyAttribute in rulePropertyAttributes)
                        {
                            if (rulePropertyAttribute.Owner.IsNull() || rulePropertyAttribute.Owner == validatableType)
                            {
                                rules.Add(CreateRule(ruleType, context, culture, contextEntry, owner, rulePropertyAttribute));
                            }
                        }
                    }
                    else
                    {
                        rules.Add(CreateRule(ruleType, context, culture, contextEntry, owner));
                    }
                }
            }

            return Task.FromResult(rules.AsEnumerable());
        }

        public async Task<IEnumerable<IRule>> ResolveRules(
            object instance, 
            object context, 
            SystemCulture culture, 
            Assembly assembly, 
            CancellationToken cancellationToken)
        {
            var rules = new List<IRule>();
            var instanceType = instance.GetType();

            foreach (var ruleClass in ResolveEntityRules(assembly, instanceType))
            {
                rules.AddRange(
                    await CreateRules(
                        context, 
                        culture, 
                        instance, 
                        instanceType, 
                        ruleClass, 
                        cancellationToken));
            }

            return rules;
        }

        public Task<IEnumerable<IRule>> ResolveRules(
            object instance, 
            object context, 
            SystemCulture culture, 
            CancellationToken cancellationToken)
        {
            return ResolveRules(
                instance, 
                context, 
                culture, 
                instance.GetType().Assembly, 
                cancellationToken);
        }

        #endregion

        #region Private Methods

        private static IList<RuleTypeEntry> ResolveEntityRules(
            Assembly assembly, 
            Type entityType)
        {
            if (!entityRules.ContainsKey(entityType))
            {
                lock (resolveEntityRulesLock)
                {
                    if (!entityRules.ContainsKey(entityType))
                    {
                        entityRules.Add(entityType, new Dictionary<Assembly, IList<RuleTypeEntry>>());
                    }
                }
            }

            if (!entityRules[entityType].ContainsKey(assembly))
            {
                lock (resolveEntityRulesLock)
                {
                    if (!entityRules[entityType].ContainsKey(assembly))
                    {
                        var assemblyEntries = ResolveRuleEntries(assembly);

                        var ruleEntries = new List<RuleTypeEntry>();

                        foreach (var ruleEntry in assemblyEntries)
                        {
                            if (ruleEntry.RuleAttribute.EntityType.IsNull() ||
                            ruleEntry.RuleAttribute.EntityType.Equals(entityType) ||
                            entityType.IsSubclassOf(ruleEntry.RuleAttribute.EntityType))
                            {
                                ruleEntries.Add(ruleEntry);
                            }
                        }

                        entityRules[entityType].Add(assembly, ruleEntries);
                    }
                }
            }

            return entityRules[entityType][assembly];
        }

        private static IList<RuleTypeEntry> ResolveRuleEntries(Assembly assembly)
        {
            if (!assemblyRules.ContainsKey(assembly))
            {
                lock (resolveRulesLock)
                {
                    if (!assemblyRules.ContainsKey(assembly))
                    {
                        var iRuleType = typeof(IRule);

                        var ruleTypeList = new List<RuleTypeEntry>();

                        foreach (var ruleType in assembly.GetTypes().Where(t => iRuleType.IsAssignableFrom(t) && !t.IsAbstract))
                        {
                            var ruleEntry = RuleTypeEntry.CreateEntry(ruleType);

                            if (ruleEntry.IsNotNull())
                            {
                                ruleTypeList.Add(ruleEntry);
                            }
                        }

                        assemblyRules.Add(assembly, ruleTypeList);
                    }
                }
            }

            return assemblyRules[assembly];
        }

        private IRule CreateRule(
            Type ruleType, 
            object context, 
            SystemCulture culture, 
            RuleContextAttribute contextType, 
            object owner)
        {
            var rule = (IRule)_serviceProvider.GetService(ruleType);
            rule.Context = context;
            rule.Culture = culture;
            rule.Owner = owner;
            rule.Severity = contextType.IsNull() ? SeverityType.Critical : contextType.Severity;
            rule.Name = ruleType.Name;
            return rule;
        }

        private IRule CreateRule(
            Type ruleType, 
            object context, 
            SystemCulture culture, 
            RuleContextAttribute contextType, 
            object owner, 
            RulePropertyAttribute propertyType)
        {
            var rule = CreateRule(ruleType, context, culture, contextType, owner);
            rule.IsPropertyRule = true;
            rule.PropertyName = propertyType.PropertyName;
            rule.DisplayName = propertyType.DisplayName;
            rule.ReadOnlyMessage = propertyType.ReadOnlyMessage;
            return rule;
        }

        private static async Task<Notification> ValidateRule(
            IRule rule, 
            IEnumerable<IApplicableRule> applicableRules, 
            IEnumerable<IRequiredRule> requiredRules,
            CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            if (!applicableRules.Contains(r => r.PropertyName == rule.PropertyName && r.Owner == rule.Owner)
                    && !requiredRules.Contains(r => r.PropertyName == rule.PropertyName && r.Owner == rule.Owner))
            {
                if (rule.MustValidate())
                {
                    notification += await rule.Validate(cancellationToken);
                }
            }

            return notification;
        }

        #endregion
    }
}

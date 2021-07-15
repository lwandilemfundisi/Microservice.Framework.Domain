using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.RuleValidator
{
    public class Validator : IValidator
    {
        private static object syncer = new object();
        private readonly IServiceProvider _serviceProvider;

        #region Constructors

        public Validator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Private Members

        private static ConcurrentDictionary<Type, IList<Type>> entityRules = new ConcurrentDictionary<Type, IList<Type>>();

        #endregion

        #region Public Methods

        public async Task<ConcurrentDictionary<Type, Notification>> ValidateEntityAsync<TObject>(
            TObject objectToValidate, 
            CancellationToken cancellationToken)
        {
            await FetchRules<TObject>();
            return await ValidateRulesAsync(objectToValidate, cancellationToken);
        }

        #endregion

        #region Private Methods

        private Task FetchRules<TObject>()
        {
            return Task.Run(() =>
            {
                if (!entityRules.ContainsKey(typeof(TObject)))
                {
                    lock (syncer)
                    {
                        if (!entityRules.ContainsKey(typeof(TObject)))
                        {
                            entityRules.SafeAddKey(typeof(TObject), new List<Type>());

                            //I shouldn't get all assemblies, this is not efficient
                            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                            foreach (var assembly in allAssemblies.Where(c => !c.FullName.Contains("NHibernate")).AsEnumerable())
                            {
                                var typeTRules = assembly
                                                .GetTypes()
                                                .AsEnumerable()
                                                .Where(t => typeof(IRule<TObject>).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

                                foreach (var rule in typeTRules)
                                {
                                    entityRules[typeof(TObject)].Add(rule);
                                }
                            }
                        }
                    }
                }
            });
        }

        private Task<ConcurrentDictionary<Type, Notification>> ValidateRulesAsync<TObject>(
            TObject objectToValidate, 
            CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                var noticationDictionary = new ConcurrentDictionary<Type, Notification>();

                Parallel.ForEach(entityRules[typeof(TObject)], (rule) =>
                {
                    var notification = NotificationHelper.CreateNotification(typeof(TObject), rule);

                    noticationDictionary.TryAdd(rule, notification);

                    var actualRules = CreatePropertyRules(rule, objectToValidate);

                    Parallel.ForEach(actualRules, (actualRule) =>
                    {
                        if (actualRule.MustValidate())
                        {
                            var notificationMessage = actualRule.Validate();
                            if (notificationMessage.IsNotNull())
                            {
                                notification.Append(notificationMessage);
                            }
                        }
                    });
                });

                return noticationDictionary;
            }, cancellationToken);
        }

        public IList<IRule<TObject>> CreatePropertyRules<TObject>(
            Type ruleType, 
            TObject objectToValidate)
        {
            var propertyRules = new List<IRule<TObject>>();

            var customAttributes = ruleType.CustomAttributes;

            foreach(var customAttribute in customAttributes)
            {
                if (customAttribute.ConstructorArguments.HasItems())
                {
                    var propertyNameToValidate = customAttribute.ConstructorArguments.FirstOrDefault();
                    var rule = _serviceProvider.GetService(ruleType) as IRule<TObject>;
                    rule.Owner = objectToValidate;
                    rule.Property = objectToValidate.GetType().GetProperty(propertyNameToValidate.Value.ToString());
                    propertyRules.Add(rule);
                }
            }

            return propertyRules;
        }

        #endregion
    }
}

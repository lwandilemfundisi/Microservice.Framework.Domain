using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class Rule : IRule
    {
        private bool getPropertyValueCalled;
        private static Type stringType = typeof(string);
        private static Type enumerableType = typeof(IEnumerable);
        private object propertyValue;
        private PropertyInfo propertyType;
        private Type propertyValueType;
        private bool? propertyHasValue;

        #region IRule Members

        public object Context { get; set; }

        public SystemCulture Culture { get; set; }
       
        public object Owner { get; set; }

        public bool IsPropertyRule { get; set; }

        public string PropertyName { get; set; }

        public string Name { get; set; }

        public string ReadOnlyMessage { get; set; }

        public Task<Notification> Validate(CancellationToken cancellationToken)
        {
            return OnValidate(cancellationToken);
        }

        public bool IsApplicabilityRule { get; set; }

        public SeverityType Severity { get; set; }

        public bool MustValidate()
        {
            return OnMustValidate();
        }

        public string DisplayName { get; set; }
        
        public bool CanExecuteParallel()
        {
            return OnCanExecuteParallel();
        }

        #endregion

        #region Properties

        protected Type PropertyValueType
        {
            get
            {
                if (propertyValueType.IsNull() && PropertyValue.IsNotNull())
                {
                    propertyValueType = PropertyValue.GetType();
                }

                return propertyValueType;
            }
        }

        protected PropertyInfo PropertyType
        {
            get
            {
                if (propertyType.IsNull())
                {
                    var instanceType = Owner.GetType();
                    propertyType = instanceType.GetProperty(PropertyName);
                    Invariant.IsFalse(propertyType.IsNull(), () => "No property {0} exists on {1}".FormatInvariantCulture(PropertyName, instanceType.FullName));
                }

                return propertyType;
            }
        }

        protected virtual object PropertyValue
        {
            get
            {
                if (!getPropertyValueCalled)
                {
                    getPropertyValueCalled = true;
                    propertyValue = PropertyType.GetValue(Owner, null);
                }

                return propertyValue;
            }
        }

        #endregion

        #region Methods

        protected bool PropertyHasValue()
        {
            if (!propertyHasValue.HasValue)
            {
                propertyHasValue = false;

                if (PropertyValue.IsNotNull())
                {
                    if (PropertyType.PropertyType.Equals(stringType))
                    {
                        propertyHasValue = PropertyValue.AsString().IsNotNullOrEmpty();
                    }
                    else if (enumerableType.IsAssignableFrom(PropertyType.PropertyType))
                    {
                        var list = PropertyValue as IList;
                        if (list.IsNotNull())
                        {
                            propertyHasValue = list.Count > 0;
                        }
                    }
                    else
                    {
                        propertyHasValue = true;
                    }
                }
            }

            return propertyHasValue.Value;
        }

        protected virtual Message CreateMessage(string message, params object[] values)
        {
            var ruleMessage = ValidationHelper.CreateMessage(this, GenerateMessagePropertyName(), message, GenerateTag(Owner), values);
            if (ReadOnlyMessage.IsNotNullOrEmpty())
            {
                ruleMessage.Text = ReadOnlyMessage;
                ruleMessage.MayOverrideMessage = false;
            }
            return ruleMessage;
        }

        protected virtual Message CreateReadOnlyMessage(string message, params object[] values)
        {
            var ruleMessage = CreateMessage(message, values);
            ruleMessage.MayOverrideMessage = false;
            return ruleMessage;
        }

        #endregion

        #region Virtual Methods

        protected virtual bool OnMustValidate()
        {
            return true;
        }

        protected virtual Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            return Task.FromResult(Notification.CreateEmpty());
        }

        protected virtual string GenerateMessagePropertyName()
        {
            return PropertyName;
        }

        protected virtual string GenerateTag(object instance)
        {
            //var taggable = instance as ITaggable;
            //if (taggable.IsNotNull())
            //{
            //    return taggable.GenerateTag();
            //}
            return string.Empty;
        }

        protected virtual bool OnCanExecuteParallel()
        {
            return true;
        }

        #endregion
}

    public abstract class Rule<T> : Rule
    {
        #region Properties

        protected T Instance
        {
            get
            {
                return (T)Owner;
            }
        }

        #endregion
    }

    public abstract class Rule<T, C> : Rule<T>
        where T : class
        where C : class
    {
        #region Methods

        public C GetContext()
        {
            return (C)Context;
        }

        #endregion
    }

    public abstract class ValidationRule<T, C, Z> : Rule<T>
        where T : class
        where C : class
        where Z : SystemCulture
    {
        #region Methods

        public C GetContext()
        {
            return (C)Context;
        }

        public Z GetCulture()
        {
            return (Z)Culture;
        }
        #endregion
    }
}

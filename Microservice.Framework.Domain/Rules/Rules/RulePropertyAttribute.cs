using Microservice.Framework.Common;
using System;

namespace Microservice.Framework.Domain.Rules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RulePropertyAttribute : Attribute
    {
        #region Constructors

        public RulePropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
            DisplayName = propertyName.RemoveCamelCasing();
        }

        public RulePropertyAttribute(string propertyName, Type owner)
        {
            PropertyName = propertyName;
            DisplayName = propertyName.RemoveCamelCasing();
            Owner = owner;
        }

        public RulePropertyAttribute(string propertyName, string displayName)
        {
            PropertyName = propertyName;
            DisplayName = displayName;
        }

        #endregion

        #region Properties

        public string PropertyName { get; private set; }

        public Type Owner { get; private set; }

        public string DisplayName { get; private set; }

        public string ReadOnlyMessage { get; set; }

        #endregion
    }
}

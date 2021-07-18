using System;

namespace Microservice.Framework.Domain.Rules
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RuleAttribute : Attribute
    {
        #region Constructors

        public RuleAttribute(Type entityType, Type entityOwnerType)
        {
            EntityType = entityType;
            EntityOwnerType = entityOwnerType;
        }

        public RuleAttribute(Type entityType)
        {
            EntityType = entityType;
        }

        public RuleAttribute()
        {
        }

        #endregion

        #region Public Properties

        public Type EntityType { get; private set; }

        public Type EntityOwnerType { get; private set; }

        #endregion
    }
}
using Microservice.Framework.Domain.Rules.Notifications;
using System;

namespace Microservice.Framework.Domain.Rules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RuleContextAttribute : Attribute
    {
        #region Constructors

        public RuleContextAttribute(Type contextType)
            : this(contextType,SeverityType.Critical)
        {
        }

        public RuleContextAttribute(Type contextType,SeverityType severity)
        {
            ContextType = contextType;
            Severity = severity;
        }

        #endregion

        #region Properties

        public Type ContextType { get; private set; }

        public SeverityType Severity { get; private set; }

        #endregion
    }
}

using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.RuleValidator
{
    public class RuleTypeEntry
    {
        #region Properties

        public Type RuleType { get; set; }

        public RuleAttribute RuleAttribute { get; set; }

        public IList<RuleContextAttribute> RuleContexts { get; set; }

        public IList<RulePropertyAttribute> RuleProperties { get; set; }

        #endregion

        #region Methods

        public static RuleTypeEntry CreateEntry(Type ruleType)
        {
            var ruleEntry = new RuleTypeEntry();

            ruleEntry.RuleType = ruleType;
            ruleEntry.RuleAttribute = ruleType.GetCustomAttributes(typeof(RuleAttribute), false).FirstOrDefault() as RuleAttribute;

            if (ruleEntry.RuleAttribute.IsNotNull())
            {
                ruleEntry.RuleContexts = new List<RuleContextAttribute>();
                ruleType.GetCustomAttributes(typeof(RuleContextAttribute), false).ForEach(rc => ruleEntry.RuleContexts.Add((RuleContextAttribute)rc));

                ruleEntry.RuleProperties = new List<RulePropertyAttribute>();
                ruleType.GetCustomAttributes(typeof(RulePropertyAttribute), false).ForEach(rc => ruleEntry.RuleProperties.Add((RulePropertyAttribute)rc));

                return ruleEntry;
            }

            return null;
        }

        #endregion
    }
}

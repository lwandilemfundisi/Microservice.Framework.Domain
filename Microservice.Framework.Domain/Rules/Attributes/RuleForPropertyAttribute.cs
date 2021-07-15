using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RuleForPropertyAttribute : Attribute
    {
        #region Constructors

        public RuleForPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        #endregion

        #region Properties

        public string PropertyName { get; set; }

        #endregion
    }
}

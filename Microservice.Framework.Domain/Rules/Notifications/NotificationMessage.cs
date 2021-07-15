using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.Notifications
{
    public class NotificationMessage
    {
        #region Constructors

        public NotificationMessage()
        {
            Construct();
        }

        #endregion

        #region Properties

        public PropertyInfo Property { get; set; }

        public string Message { get; set; }

        public NotificationMessageType NotificationMessageType { get; set; }

        #endregion

        #region Private Methods

        private void Construct()
        {

        }

        #endregion
    }
}

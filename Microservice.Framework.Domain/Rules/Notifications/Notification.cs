using Microservice.Framework.Common;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Microservice.Framework.Domain.Rules.Notifications
{
    public class Notification
    {
        #region Constructors

        public Notification(Type entityType, Type ruleType)
        {
            Construct(entityType, ruleType);
        }

        #endregion

        #region Properties

        public Type EntityType { get; set; }

        public Type RuleType { get; set; }

        public DateTime OccuredOn { get; private set; }

        public bool HasErrors
        {
            get 
            {
                if(Messages.HasItems())
                {
                    return Messages.Where(m => m.NotificationMessageType == NotificationMessageType.Error).HasItems();
                }

                return false;
            }
        }

        public bool HasWarnings
        {
            get
            {
                if (Messages.HasItems())
                {
                    return Messages.Where(m => m.NotificationMessageType == NotificationMessageType.Warning).HasItems();
                }

                return false;
            }
        }

        public bool HasInformation
        {
            get
            {
                if (Messages.HasItems())
                {
                    return Messages.Where(m => m.NotificationMessageType == NotificationMessageType.Information).HasItems();
                }

                return false;
            }
        }

        public ConcurrentQueue<NotificationMessage> Messages { get; set; }

        #endregion

        #region Private Methods

        private void Construct(Type entityType, Type ruleType)
        {
            EntityType = entityType;
            RuleType = ruleType;
            OccuredOn = DateTime.Now;
            Messages = new ConcurrentQueue<NotificationMessage>();
        }

        #endregion
    }
}

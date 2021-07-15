using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class NotificationHelper
    {
        #region Syncer

        static object sync = new object();

        #endregion

        public static Notification Append(this Notification notification, NotificationMessage message)
        {
            notification.Messages.Enqueue(message);
            return notification;
        }

        public static NotificationMessage CreateNotificationMessage(string message, PropertyInfo property, NotificationMessageType notificationMessageType)
        {
            return new NotificationMessage()
            {
                Message = message,
                Property = property,
                NotificationMessageType = notificationMessageType
            };
        }

        public static Notification CreateNotification(Type entityType, Type ruleType)
        {
            return new Notification(entityType, ruleType);
        }

        public static bool HasErrors(this ConcurrentDictionary<Type, Notification> notifications)
        {
            return notifications.Values.AsEnumerable().Where(r => r.HasErrors == true).HasItems();
        }

        public static bool HasWarnings(this ConcurrentDictionary<Type, Notification> notifications)
        {
            return notifications.Values.AsEnumerable().Where(r => r.HasWarnings == true).HasItems();
        }

        public static bool HasInformation(this ConcurrentDictionary<Type, Notification> notifications)
        {
            return notifications.Values.AsEnumerable().Where(r => r.HasInformation == true).HasItems();
        }

        public static ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>> HandleResult(this ConcurrentDictionary<Type, Notification> notifications)
        {
            var dictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>>();
            
            notifications.ForEach((_not) => 
            {
                var errors = _not.Value.Messages.Where(m => m.NotificationMessageType == NotificationMessageType.Error);
                var warnings = _not.Value.Messages.Where(m => m.NotificationMessageType == NotificationMessageType.Warning);
                var information = _not.Value.Messages.Where(m => m.NotificationMessageType == NotificationMessageType.Information);

                if (errors.HasItems())
                {
                    dictionary["errors"] = null;
                    SafeHandleNotifications(dictionary, "errors", errors);
                }

                if (warnings.HasItems())
                {
                    dictionary["warnings"] = null;
                    SafeHandleNotifications(dictionary, "warnings", warnings);
                }

                if (information.HasItems())
                {
                    dictionary["information"] = null;
                    SafeHandleNotifications(dictionary, "information", information);
                }
            });

            return dictionary;
        }

        #region Private Methods

        private static void SafeHandleNotifications(ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>> dictionary, string key, IEnumerable<NotificationMessage> categoryMessages)
        {
            dictionary[key] = new ConcurrentDictionary<string, List<string>>();

            categoryMessages.ForEach((cat) => 
            {
                dictionary[key].TryAdd(cat.Property.Name, new List<string>());
            });

            categoryMessages.ForEach((cat) =>
            {
                lock (sync)
                {
                    dictionary[key][cat.Property.Name].Add(cat.Message);
                }
            });
        }

        #endregion
    }
}

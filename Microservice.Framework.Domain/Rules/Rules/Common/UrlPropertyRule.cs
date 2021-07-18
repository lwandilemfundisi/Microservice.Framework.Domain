﻿using Microservice.Framework.Common;
using Microservice.Framework.Domain.Rules.Notifications;
using System;

namespace Microservice.Framework.Domain.Rules.Common
{
    public abstract class UrlPropertyRule<T> : Rule<T> where T : class
    {
        #region Virtual Methods

        protected override Notification OnValidate()
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            if (propertyValue.IsNotNullOrEmpty())
            {
                Uri uri = null;

                try
                {
                    uri = new Uri(propertyValue);
                }
                catch(UriFormatException)
                {
                }

                if (uri.IsNull() || uri.Scheme.IsNullOrEmpty() || uri.Authority.IsNullOrEmpty())
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return notification;
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} is an invalid url format", DisplayName);
        }

        #endregion
    }
}

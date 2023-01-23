using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microservice.Framework.Common;

namespace Microservice.Framework.Domain.Rules.Notifications
{
    public sealed class Notification 
        : IFormattable, IEnumerable<Message>
    {
        #region Constructors

        public Notification()
        {
            Messages = new List<Message>();
        }

        #endregion

        #region Properties

        public IList<Message> Messages { get; set; }

        public bool HasMessages => Messages.Count > 0;

        public bool HasErrors => Messages.Contains(m => m.Severity == SeverityType.Critical);

        public bool HasWarnings => Messages.Contains(m => m.Severity == SeverityType.Warning);

        public bool HasInformation => Messages.Contains(m => m.Severity == SeverityType.Information);

        #endregion

        #region Methods

        public IEnumerable<Message> Warnings()
        {
            return Messages.Where(m => m.Severity == SeverityType.Warning);
        }

        public void MakeWarningsErrors()
        {
            foreach (var warning in Warnings())
            {
                warning.Severity = SeverityType.Critical;
            }
        }

        public IEnumerable<Message> GetErrors()
        {
            return Messages.Where(m => m.Severity == SeverityType.Critical);
        }

        public void Clear()
        {
            Messages.Clear();
        }

        public static Notification CreateEmpty()
        {
            return new Notification();
        }

        public static Notification Create(Message message)
        {
            Notification notification = CreateEmpty();
            notification.AddMessage(message);
            return notification;
        }

        public static Notification Create(IList<Message> messages)
        {
            Notification notification = CreateEmpty();
            notification.AddMessage(messages);
            return notification;
        }

        public void AddMessage(Message message)
        {
            if (message != null)
            {
                Messages.Add(message);
            }
        }

        public void AddWarning(Message message)
        {
            if (message != null)
            {
                message.Severity = SeverityType.Warning;
                Messages.Add(message);
            }
        }

        public void AddError(Message message)
        {
            if (message != null)
            {
                message.Severity = SeverityType.Critical;
                Messages.Add(message);
            }
        }

        public void AddMessage(Message message, int index)
        {
            if (message != null)
            {
                Messages.Insert(index, message);
            }
        }

        public void AddMessage(IEnumerable<Message> messageList)
        {
            if (messageList != null)
            {
                foreach (Message message in messageList)
                {
                    Messages.Add(message);
                }
            }
        }

        public string ToString(bool detailed)
        {
            return ToString(detailed, Environment.NewLine);
        }

        public string ToString(bool detailed, string separator)
        {
            var builder = new StringBuilder();
            foreach (Message message in Messages)
            {
                builder.Append(string.Format(
                    CultureInfo.InvariantCulture, 
                    "{0}{1}", 
                    builder.Length > 0 ? separator : null, 
                    message.ToString(detailed)));
            }
            return builder.ToString();
        }

        public void Verify()
        {
            Invariant.IsFalse(HasErrors, () => ToString(true));
        }

        public void Verify(Func<string> description)
        {
            Invariant.IsFalse(
                HasErrors, 
                () => "{0}: {1}".FormatInvariantCulture(description(), 
                ToString()));
        }

        public string ToString(string separator)
        {
            return ToString(false, separator);
        }

        #endregion

        #region Operator Overloads

        public static Notification Add(Notification result1, Notification result2)
        {
            return result1 + result2;
        }

        public static Notification operator +(Notification result1, Notification result2)
        {
            Invariant.ArgumentNotNull(result1, () => "result1");

            if (result2 != null)
            {
                result1.AddMessage(result2);
            }
            return result1;
        }

        public static Notification operator +(Notification result1, Message result2)
        {
            Invariant.ArgumentNotNull(result1, () => "result1");

            if (result2 != null)
            {
                result1.AddMessage(result2);
            }
            return result1;
        }

        public static implicit operator string(Notification notification)
        {
            return notification.ToString();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator<Message> IEnumerable<Message>.GetEnumerator()
        {
            for (int i = 0; i < Messages.Count; i++)
            {
                yield return Messages[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Message>)this).GetEnumerator();
        }

        #endregion

        #region Virtual Methods

        public override string ToString()
        {
            return ToString(false);
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(false);
        }

        #endregion
    }
}
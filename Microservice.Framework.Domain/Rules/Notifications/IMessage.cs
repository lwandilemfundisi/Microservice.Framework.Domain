using System.Collections.Generic;

namespace Microservice.Framework.Domain.Rules.Notifications
{
    public interface IMessage
    {
        string Text { get; set; }

        SeverityType Severity { get; set; }

        string SystemReference { get; set; }

        string Tag { get; set; }

        string PropertyName { get; set; }

        string RuleInfo { get; set; }

        MessageType MessageType { get; set; }

        IDictionary<string, IEnumerable<string>> ClassRuleTags { get; set; }
    }
}

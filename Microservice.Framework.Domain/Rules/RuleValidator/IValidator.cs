using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Rules.RuleValidator
{
    public interface IValidator
    {
        Task<ConcurrentDictionary<Type, Notification>> ValidateEntityAsync<TObject>(
            TObject objectToValidate,
            CancellationToken cancellationToken);
    }
}

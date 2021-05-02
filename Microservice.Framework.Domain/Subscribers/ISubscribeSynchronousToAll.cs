using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Events;

namespace Microservice.Framework.Domain.Subscribers
{
    public interface ISubscribeSynchronousToAll
    {
        Task HandleAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken);
    }
}
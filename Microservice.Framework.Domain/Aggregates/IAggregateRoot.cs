using Microservice.Framework.Common;
using Microservice.Framework.Domain.Events;
using Microservice.Framework.Domain.Events.Serializers;
using Microservice.Framework.Persistence;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Aggregates
{
    public interface IAggregateRoot
    {
        IAggregateName Name { get; }

        int Version { get; set; }

        bool IsNew { get; }

        Task<IReadOnlyCollection<IDomainEvent>> CommitAsync(
            IEventJsonSerializer _eventJsonSerializer, 
            ISourceId sourceId, 
            CancellationToken cancellationToken);

        bool HasSourceId(ISourceId sourceId);

        IIdentity GetIdentity();

        IAggregateRoot AsExisting();
        
        IPersistence Persistence { get; set; }

    }

    public interface IAggregateRoot<TIdentity> : IAggregateRoot
        where TIdentity : IIdentity
    {
        TIdentity Id { get; set; }
    }
}

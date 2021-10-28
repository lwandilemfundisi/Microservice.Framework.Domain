using Newtonsoft.Json;
using Microservice.Framework.Common;
using Microservice.Framework.Domain.Events;
using Microservice.Framework.Domain.Events.AggregateEvents;
using Microservice.Framework.Domain.Events.Serializers;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Persistence;
using Microservice.Framework.Persistence.Queries.Filtering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Aggregates
{
    public abstract class AggregateRoot<TAggregate, TIdentity> : IAggregateRoot<TIdentity>
        where TAggregate : AggregateRoot<TAggregate, TIdentity>
        where TIdentity : IIdentity
    {
        private bool _exists;
        private readonly List<IOccuredEvent> _occuredEvents = new List<IOccuredEvent>();
        private static readonly IAggregateName AggregateName = typeof(TAggregate).GetAggregateName();
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(10);

        protected AggregateRoot(TIdentity id)
        {
            if ((this as TAggregate) == null)
            {
                throw new InvalidOperationException(
                    $"Aggregate '{GetType().PrettyPrint()}' specifies '{typeof(TAggregate).PrettyPrint()}' as generic argument, it should be its own type");
            }

            Id = id;
        }

        public TIdentity Id { get; set; }

        public IAggregateName Name => AggregateName;

        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<IOccuredEvent> OccuredEvents => _occuredEvents;

        public int Version { get; set; }

        public bool IsNew => Version <= 0;

        [NotMapped]
        public IPersistence Persistence { get; set; }

        public async Task<IReadOnlyCollection<IDomainEvent>> CommitAsync(
            IEventJsonSerializer _eventJsonSerializer, 
            ISourceId sourceId, 
            CancellationToken cancellationToken)
        {
            if(!_exists)
                await Persistence.Save(this, CancellationToken.None);
            else
                await Persistence.Update(this, CancellationToken.None);

            //await Persistence.Dispose(cancellationToken);

            if (_occuredEvents.HasItems())
            {
                var domainEvents = _occuredEvents
                .Select(e =>
                {
                    return _eventJsonSerializer.Serialize(e.AggregateEvent, e.Metadata);
                })
                .Select((e, i) =>
                {
                    var committedDomainEvent = new CommittedDomainEvent
                    {
                        AggregateId = Id.Value,
                        AggregateName = e.Metadata[MetadataKeys.AggregateName],
                        AggregateSequenceNumber = e.AggregateSequenceNumber,
                        Data = e.SerializedData,
                        Metadata = e.SerializedMetadata,
                        GlobalSequenceNumber = i + 1,
                    };
                    return committedDomainEvent;
                })
                .Select(e => _eventJsonSerializer.Deserialize<TAggregate, TIdentity>(Id, e))
                .ToList();

                _occuredEvents.Clear();

                return domainEvents;
            }
            else 
            {
                _occuredEvents.Clear();
                return new IDomainEvent<TAggregate, TIdentity>[] { };
            }
        }

        public IIdentity GetIdentity()
        {
            return Id;
        }

        public bool HasSourceId(ISourceId sourceId)
        {
            return !sourceId.IsNone() && _previousSourceIds.Any(s => s.Value == sourceId.Value);
        }

        protected virtual void Emit<TEvent>(TEvent aggregateEvent, IMetadata metadata = null)
            where TEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }

            var aggregateSequenceNumber = Version + 1;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.Now;
            var eventMetadata = new Metadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = Name.Value,
                AggregateId = Id.Value,
                EventId = eventId
            };
            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }

            _occuredEvents.Add(new OccuredEvent(aggregateEvent, eventMetadata));

            Version++;
        }

        public IAggregateRoot AsExisting()
        {
            _exists = true;
            return this;
        }

        private class CommittedDomainEvent : ICommittedDomainEvent
        {
            public long GlobalSequenceNumber { get; set; }
            public string AggregateId { get; set; }
            public string AggregateName { private get; set; }
            public string Data { get; set; }
            public string Metadata { get; set; }
            public int AggregateSequenceNumber { get; set; }

            public override string ToString()
            {
                return new StringBuilder()
                    .AppendLineFormat("{0} v{1} ==================================", AggregateName,
                        AggregateSequenceNumber)
                    .AppendLine(PrettifyJson(Metadata))
                    .AppendLine("---------------------------------")
                    .AppendLine(PrettifyJson(Data))
                    .Append("---------------------------------")
                    .ToString();
            }

            private static string PrettifyJson(string json)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject(json);
                    var prettyJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
                    return prettyJson;
                }
                catch (Exception)
                {
                    return json;
                }
            }
        }
    }
}

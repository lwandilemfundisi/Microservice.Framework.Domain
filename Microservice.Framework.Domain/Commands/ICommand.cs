using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.ExecutionResults;
using Microservice.Framework.Common;
using Microservice.Framework.VersionTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Commands
{
    public interface ICommand : IVersionedType
    {
        Task<IExecutionResult> PublishAsync(ICommandBus commandBus, CancellationToken cancellationToken);
        ISourceId GetSourceId();
    }

    public interface ICommand<in TAggregate, out TIdentity, TResult> : ICommand
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TResult : IExecutionResult
    {
        TIdentity AggregateId { get; }
        ISourceId SourceId { get; }
    }
}

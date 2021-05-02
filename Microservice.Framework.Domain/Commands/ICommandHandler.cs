using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.ExecutionResults;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Commands
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in TAggregate, TIdentity, TResult, in TCommand> : ICommandHandler
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TResult : IExecutionResult
        where TCommand : ICommand<TAggregate, TIdentity, TResult>
    {
        Task<TResult> ExecuteCommandAsync(TAggregate aggregate, TCommand command, CancellationToken cancellationToken);
    }
}

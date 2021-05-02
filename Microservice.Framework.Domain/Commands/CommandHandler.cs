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
    public abstract class CommandHandler<TAggregate, TIdentity, TResult, TCommand> :
        ICommandHandler<TAggregate, TIdentity, TResult, TCommand>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TResult : IExecutionResult
        where TCommand : ICommand<TAggregate, TIdentity, TResult>
    {
        public abstract Task<TResult> ExecuteCommandAsync(
            TAggregate aggregate,
            TCommand command,
            CancellationToken cancellationToken);
    }

    public abstract class CommandHandler<TAggregate, TIdentity, TCommand> :
        CommandHandler<TAggregate, TIdentity, IExecutionResult, TCommand>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate, TIdentity, IExecutionResult>
    {
        public override async Task<IExecutionResult> ExecuteCommandAsync(
            TAggregate aggregate,
            TCommand command,
            CancellationToken cancellationToken)
        {
            await ExecuteAsync(aggregate, command, cancellationToken).ConfigureAwait(false);
            return ExecutionResult.Success();
        }

        public abstract Task ExecuteAsync(
            TAggregate aggregate,
            TCommand command,
            CancellationToken cancellationToken);
    }
}

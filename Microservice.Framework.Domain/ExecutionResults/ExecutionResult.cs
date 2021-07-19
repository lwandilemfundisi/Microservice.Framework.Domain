using Microservice.Framework.Domain.Rules.Notifications;
using System.Collections.Generic;
using System.Linq;

namespace Microservice.Framework.Domain.ExecutionResults
{
    public abstract class ExecutionResult : IExecutionResult
    {
        private static readonly IExecutionResult SuccessResult = new SuccessExecutionResult();
        private static readonly IExecutionResult FailedResult = new FailedExecutionResult(Notification.CreateEmpty());

        public static IExecutionResult Success() => SuccessResult;

        public static IExecutionResult Success<TResult>(TResult result) => new SuccessExecutionResult<TResult>(result);

        public static IExecutionResult Failed() => FailedResult;

        public static IExecutionResult Failed(Notification notification) => new FailedExecutionResult(notification);

        public abstract bool IsSuccess { get; }

        public override string ToString()
        {
            return $"ExecutionResult - IsSuccess:{IsSuccess}";
        }
    }
}
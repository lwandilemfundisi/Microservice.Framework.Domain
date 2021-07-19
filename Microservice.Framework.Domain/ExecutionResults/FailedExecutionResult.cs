using Microservice.Framework.Domain.Rules.Notifications;
using System.Collections.Generic;
using System.Linq;

namespace Microservice.Framework.Domain.ExecutionResults
{
    public class FailedExecutionResult : ExecutionResult
    {
        public Notification Errors { get; }

        public FailedExecutionResult(Notification errors)
        {
            Errors = errors;
        }
            
        public override bool IsSuccess { get; } = false;

        public override string ToString()
        {
            return Errors.HasErrors
                ? $"Failed execution due to: {Errors}"
                : "Failed execution";
        }
    }
}
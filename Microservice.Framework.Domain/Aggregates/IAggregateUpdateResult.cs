using Microservice.Framework.Domain.ExecutionResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Aggregates
{
    public interface IAggregateUpdateResult<out TExecutionResult>
        where TExecutionResult : IExecutionResult
    {
        TExecutionResult Result { get; }
    }
}

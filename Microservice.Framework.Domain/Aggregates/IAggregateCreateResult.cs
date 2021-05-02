using Microservice.Framework.Domain.ExecutionResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Aggregates
{
    public interface IAggregateCreateResult<out TExecutionResult>
        where TExecutionResult : IExecutionResult
    {
        TExecutionResult Result { get; }
    }

    public class AggregateCreateResult<TExecutionResult> : IAggregateCreateResult<TExecutionResult>
            where TExecutionResult : IExecutionResult
    {
        public TExecutionResult Result { get; }

        public AggregateCreateResult(
            TExecutionResult result)
        {
            Result = result;
        }
    }
}

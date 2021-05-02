namespace Microservice.Framework.Domain.ExecutionResults
{
    public class SuccessExecutionResult : ExecutionResult
    {
        public override bool IsSuccess { get; } = true;

        public override string ToString()
        {
            return "Successful execution";
        }
    }

    public class SuccessExecutionResult<TResult> : SuccessExecutionResult
    {
        public TResult Result { get; }

        public SuccessExecutionResult(TResult result)
        {
            Result = result;
        }
    }
}
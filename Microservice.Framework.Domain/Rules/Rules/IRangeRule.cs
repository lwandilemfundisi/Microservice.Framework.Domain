namespace Microservice.Framework.Domain.Rules
{
    public interface IRangeRule : IRule
    {
        object GetMinimum();

        object GetMaximum();
    }
}
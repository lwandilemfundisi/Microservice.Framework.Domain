namespace Microservice.Framework.Domain.Rules
{
    public interface IRangeRule<T> : IRule<T>
    {
        object GetMinimum();

        object GetMaximum();
    }
}
namespace Microservice.Framework.Domain.Rules
{
    public interface IRequiredRule<T> : IRule<T>
    {
        bool IsRequired();
    }
}
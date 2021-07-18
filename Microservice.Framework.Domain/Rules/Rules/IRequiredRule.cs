namespace Microservice.Framework.Domain.Rules
{
    public interface IRequiredRule : IRule
    {
        bool IsRequired();
    }
}
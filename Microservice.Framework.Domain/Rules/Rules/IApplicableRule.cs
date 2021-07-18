namespace Microservice.Framework.Domain.Rules
{
    public interface IApplicableRule : IRule
    {
        bool IsApplicable();
    }
}

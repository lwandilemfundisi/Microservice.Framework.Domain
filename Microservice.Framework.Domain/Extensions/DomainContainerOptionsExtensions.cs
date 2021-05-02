using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerOptionsExtensions
    {
        public static IDomainContainer RegisterServices(
            this IDomainContainer domainContainer,
            Action<IServiceCollection> registerServices)
        {
            registerServices(domainContainer.ServiceCollection);
            return domainContainer;
        }
    }
}

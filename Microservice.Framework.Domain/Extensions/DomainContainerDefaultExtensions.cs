using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microservice.Framework.Domain.Extensions
{
    public static class DomainContainerDefaultExtensions
    {
        public static IDomainContainer AddDefaults(
            this IDomainContainer domainContainer,
            Assembly fromAssembly,
            Predicate<Type> predicate = null)
        {
            return domainContainer
                .AddEvents(fromAssembly, predicate)
                .AddJobs(fromAssembly, predicate)
                .AddCommands(fromAssembly, predicate)
                .AddCommandHandlers(fromAssembly, predicate)
                .AddMetadataProviders(fromAssembly, predicate)
                .AddSubscribers(fromAssembly, predicate)
                .AddQueryHandlers(fromAssembly, predicate)
                .AddRules(fromAssembly, predicate);
        }
    }
}

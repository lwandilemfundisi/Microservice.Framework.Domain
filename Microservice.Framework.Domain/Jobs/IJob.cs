using Microservice.Framework.Ioc;
using Microservice.Framework.VersionTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Jobs
{
    public interface IJob : IVersionedType
    {
        Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }
}
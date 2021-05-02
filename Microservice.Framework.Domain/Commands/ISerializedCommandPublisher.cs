using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Commands
{
    public interface ISerializedCommandPublisher
    {
        Task<ISourceId> PublishSerilizedCommandAsync(
            string name,
            int version,
            string json,
            CancellationToken cancellationToken);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Commands
{
    public interface ICommandScheduler
    {
        Task ScheduleAsync(
            ICommand command,
            DateTimeOffset runAt,
            CancellationToken cancellationToken);

        Task ScheduleAsync(
            ICommand command,
            TimeSpan delay,
            CancellationToken cancellationToken);
    }
}

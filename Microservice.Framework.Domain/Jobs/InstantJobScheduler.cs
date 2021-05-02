using Microsoft.Extensions.Logging;
using Microservice.Framework.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Jobs
{
    public class InstantJobScheduler : IJobScheduler
    {
        private readonly IJobDefinitionService _jobDefinitionService;
        private readonly IJobRunner _jobRunner;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger<InstantJobScheduler> _logger;

        public InstantJobScheduler(
            ILogger<InstantJobScheduler> logger,
            IJsonSerializer jsonSerializer,
            IJobRunner jobRunner,
            IJobDefinitionService jobDefinitionService)
        {
            _logger = logger;
            _jsonSerializer = jsonSerializer;
            _jobRunner = jobRunner;
            _jobDefinitionService = jobDefinitionService;
        }

        public async Task<IJobId> ScheduleNowAsync(IJob job, CancellationToken cancellationToken)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            var jobDefinition = _jobDefinitionService.GetDefinition(job.GetType());

            try
            {
                var json = _jsonSerializer.Serialize(job);

                if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
                {
                    _logger.LogTrace($"Executing job '{jobDefinition.Name}' v{jobDefinition.Version}: {json}");
                }

                await _jobRunner.ExecuteAsync(jobDefinition.Name, jobDefinition.Version, json, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // We want the InstantJobScheduler to behave as an out-of-process scheduler, i.e., doesn't
                // throw exceptions directly related to the job execution
                _logger.LogError(e, $"Execution of job '{jobDefinition.Name}' v{jobDefinition.Version} failed!");
            }

            return JobId.New;
        }

        public Task<IJobId> ScheduleAsync(IJob job, DateTimeOffset runAt, CancellationToken cancellationToken)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            _logger.LogWarning($"Instant scheduling configured, executing job '{job.GetType().PrettyPrint()}' NOW! Instead of at '{runAt}'");

            // Don't schedule, just execute...
            return ScheduleNowAsync(job, cancellationToken);
        }

        public Task<IJobId> ScheduleAsync(IJob job, TimeSpan delay, CancellationToken cancellationToken)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            _logger.LogWarning($"Instant scheduling configured, executing job '{job.GetType().PrettyPrint()}' NOW! Instead of in '{delay}'");

            // Don't schedule, just execute...
            return ScheduleNowAsync(job, cancellationToken);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Exceptions
{
    public class UnknownJobException : Exception
    {
        public UnknownJobException(
            string jobName,
            int jobVersion,
            string message)
            : base(message)
        {
            JobName = jobName;
            JobVersion = jobVersion;
        }

        public string JobName { get; }
        public int JobVersion { get; }

        public static UnknownJobException With(string jobName, int jobVersion)
        {
            var message = $"Job '{jobName}' v{jobVersion} is unknown. It might be one of these reasons: current software is too old, or job has been deleted";
            return new UnknownJobException(jobName, jobVersion, message);
        }
    }
}

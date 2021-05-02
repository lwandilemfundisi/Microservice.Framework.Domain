using Microservice.Framework.Common;

namespace Microservice.Framework.Domain.Jobs
{
    public class JobId : Identity<JobId>, IJobId
    {
        public JobId(string value) : base(value)
        {
        }
    }
}
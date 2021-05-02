using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events
{
    public interface IEventId : ISourceId
    {
        Guid GetGuid();
    }
}

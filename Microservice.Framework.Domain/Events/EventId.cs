using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Events
{
    public class EventId : Identity<EventId>, IEventId
    {
        public EventId(string value) : base(value)
        {
        }
    }
}

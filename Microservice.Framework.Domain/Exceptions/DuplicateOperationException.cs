using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Exceptions
{
    public class DuplicateOperationException : Exception
    {
        public ISourceId SourceId { get; }
        public IIdentity AggregateId { get; }

        public DuplicateOperationException(
            ISourceId sourceId, IIdentity aggregateId, string message)
            : base(message)
        {
            SourceId = sourceId;
            AggregateId = aggregateId;
        }
    }
}

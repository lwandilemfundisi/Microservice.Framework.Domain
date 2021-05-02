using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Framework.Domain.Exceptions
{
    public class NoCommandHandlersException : Exception
    {
        public NoCommandHandlersException(string message) : base(message)
        {
        }
    }
}

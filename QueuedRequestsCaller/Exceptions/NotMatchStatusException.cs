using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Exceptions
{
    internal class NotMatchStatusException : Exception
    {
        public int ResponseStatus { get; private set; }

        public NotMatchStatusException()
        {
        }

        public NotMatchStatusException(string message, int ResponseStatus) : base(message)
        {
            this.ResponseStatus = ResponseStatus;
        }
    }
}

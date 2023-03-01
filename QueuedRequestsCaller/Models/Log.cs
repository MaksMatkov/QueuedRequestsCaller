using QueuedRequestsCaller.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class Log
    {
        public Log(string message, RequestLogType type = RequestLogType.Info, Exception exception = null)
        {
            Type = type;
            Exception = exception;
            DateTime = DateTime.Now;
            Message = message;
        }

        public RequestLogType Type { get; }
        public Exception Exception { get; }
        public DateTime DateTime { get; }
        public string Message { get; }
    }
}

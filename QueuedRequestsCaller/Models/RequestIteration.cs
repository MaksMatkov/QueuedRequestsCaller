using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class RequestIteration
    {
        public RequestIteration(RequestModel requestModel)
        {
            RequestModel = requestModel;
            Logs = new List<Log>();
        }

        public void AddLog(Log log)
        {
            Logs.Add(log);
        }

        public RequestModel RequestModel { get; private set; }
        public List<Log> Logs { get; private set; }
    }
}

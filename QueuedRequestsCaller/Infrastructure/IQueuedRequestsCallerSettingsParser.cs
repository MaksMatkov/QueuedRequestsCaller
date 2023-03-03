using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Infrastructure
{
    public interface IQueuedRequestsCallerSettingsParser
    {
        public QueuedRequestsCallerSettings Parse(string json);
    }
}

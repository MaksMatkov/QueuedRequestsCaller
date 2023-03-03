using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class QueuedRequestsCallerSettings
    {
        public List<QueuedRequestItem> RequestsList { get; set; } = new List<QueuedRequestItem>();
        public bool DropOnReExecuteError { get; set; } = false;
    }
}

using QueuedRequestsCaller.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class QueuedRequestItem
    {
        public RequestModel Model { get; set; }
        public List<MapCouple> MappingList { get; set; } = new List<MapCouple>();

        /// <summary>
        /// Actions that take first argument this request and second parameter as next request
        /// </summary>
        public List<Action<RequestModel, RequestModel>> PostRequestActionsList { get; set; } = new List<Action<RequestModel, RequestModel>>();
    }
}

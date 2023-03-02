using QueuedRequestsCaller.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class RequestValue
    {
        public MappingValueLocation Location { get; set; }
        /// <summary>
        /// Name or path to value location
        /// </summary>
        public string FullName {get;set;}
    }
}

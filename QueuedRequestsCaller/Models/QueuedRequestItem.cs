﻿using QueuedRequestsCaller.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class QueuedRequestItem
    {
        public RequestModel model { get; set; }
        public List<MapCouple> mappingList { get; set; }
    }
}

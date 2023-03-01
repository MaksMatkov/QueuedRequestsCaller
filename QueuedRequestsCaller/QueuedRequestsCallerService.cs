using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using RestSharp;
using System.Threading.Tasks;
using QueuedRequestsCaller.Services;

namespace QueuedRequestsCaller
{
    public class QueuedRequestsCallerService
    {
        private List<QueuedRequestItem> _list { get; set; }

        public QueuedRequestsCallerService(List<QueuedRequestItem> list)
        {
            this._list = list;
        }
        
        public RestResponse MakeRequests()
        {
            RestResponse responsePointer = new RestResponse();
            foreach(var req in _list)
            {
                req.model.MakeRequest();
                if(_list.IndexOf(req) != _list.Count - 1)
                {
                    req.MappToNext(_list[_list.IndexOf(req) + 1].model);
                }

                responsePointer = req.model.RequestResponse;
            }

            return responsePointer;
        }
    }
}

using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class RequestsCallerResult : IDisposable
    {
        public RequestsCallerResult(bool isSuccessfully, List<RequestIteration> requestIteration, Exception lastException = null)
        {
            IsSuccessfully = isSuccessfully;
            LastException = lastException;
            RequestIteration = requestIteration;
        }

        public bool IsSuccessfully { get; private set; }
        public Exception LastException { get; private set; }
        public List<RequestIteration> RequestIteration { get; private set; }

        public RestResponse Response {
            get
            {
                if (RequestIteration.Count > 0)
                    return RequestIteration.Last().RequestModel.RequestResponse;
                else
                    return null;
            }
        }

        public void Dispose()
        {
            RequestIteration.Clear();
        }

        ~RequestsCallerResult()
        {
            Dispose();
        }
    }
}

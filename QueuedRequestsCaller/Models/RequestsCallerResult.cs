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
        public RequestsCallerResult(bool isSuccessfully, List<RequestIteration> requestIterationList, Exception lastException = null)
        {
            IsSuccessfully = isSuccessfully;
            LastException = lastException;
            RequestIterationList = requestIterationList;
        }

        public bool IsSuccessfully { get; private set; }
        public Exception LastException { get; private set; }
        public List<RequestIteration> RequestIterationList { get; private set; }

        public RestResponse Response
        {
            get
            {
                if (RequestIterationList.Count > 0)
                    return RequestIterationList.Last().RequestModel.RequestResponse;
                else
                    return null;
            }
        }

        public void Dispose()
        {
            RequestIterationList.Clear();
        }

        ~RequestsCallerResult()
        {
            Dispose();
        }
    }
}

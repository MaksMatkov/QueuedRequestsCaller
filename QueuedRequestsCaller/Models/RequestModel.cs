using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class RequestModel
    {
        public RequestModel(RestSharp.Method method, string resource, Dictionary<string, string> queryParameters, Dictionary<string, string> headers, JObject body)
            : this(method, resource, queryParameters, headers)
        {
            if(body != null)
                this.RestRequest.AddJsonBody(body);
        }

        public RequestModel(RestSharp.Method method, string resource, Dictionary<string, string> queryParameters, Dictionary<string, string> headers, object body)
            : this(method, resource, queryParameters, headers)
        {
            this.RestRequest.AddJsonBody(JObject.FromObject(body));
        }

        private RequestModel(RestSharp.Method method, string resource, Dictionary<string, string> queryParameters, Dictionary<string, string> headers)
        {
            this.RestRequest.Resource = resource;
            this.RestRequest.Method = method;

            var tasksList = new Task[2];
            tasksList[0] = (Task.Run(() => {
                foreach (var h in headers)
                {
                    this.RestRequest.AddHeader(h.Key, h.Value);
                }
            }));

            tasksList[1] = (Task.Run(() => {
                foreach (var h in queryParameters)
                {
                    this.RestRequest.AddQueryParameter(h.Key, h.Value);
                }
            }));

            Task.WaitAll(tasksList);
        }

        private JObject _RequestBody { get; set; }
        public JObject RequestBody
        {
            get
            {
                return _RequestBody;
            }

            set
            {
                _RequestBody = value;
                RestRequest.AddJsonBody("");
                RestRequest.AddJsonBody(value);
            }
        }
        public RestRequest RestRequest { get; private set; } = new RestRequest();
        public RestResponse RequestResponse { get; private set; }

        public RestResponse MakeRequest()
        {
            var client = new RestClient();

            RequestResponse = client.Execute(this.RestRequest);

            return RequestResponse;
        }
    }
}

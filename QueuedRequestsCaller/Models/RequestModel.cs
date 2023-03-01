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
            Body = body;
        }

        public RequestModel(RestSharp.Method method, string resource, Dictionary<string, string> queryParameters, Dictionary<string, string> headers, object body)
            : this(method, resource, queryParameters, headers)
        {
            Body = JObject.FromObject(body);
        }

        private RequestModel(RestSharp.Method method, string resource, Dictionary<string, string> queryParameters, Dictionary<string, string> headers)
        {
            Method = method;
            Resource = resource;
            QueryParameters = queryParameters;
            Headers = headers;
        }

        public RestSharp.Method Method { get; private set; }
        public string Resource { get; private set; }
        public Dictionary<string, string> QueryParameters { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public JObject Body { get; set; }
        public RestResponse RequestResponse { get; private set; }

        public RestResponse MakeRequest()
        {
            var tasksList = new Task[2];
            var client = new RestClient();
            var request = new RestRequest(Resource, Method);

            //add headers and body
            tasksList[0] = (Task.Run(() => {
                foreach (var h in Headers)
                {
                    request.AddHeader(h.Key, h.Value);
                }

                if (Body != null)
                    request.AddJsonBody(Body);
            }));

            //add query params
            tasksList[1] = (Task.Run(() => {
                foreach (var h in QueryParameters)
                {
                    request.AddQueryParameter(h.Key, h.Value);
                }
            }));

            Task.WaitAll(tasksList);

            RequestResponse = client.Execute(request);

            return RequestResponse;
        }
    }
}

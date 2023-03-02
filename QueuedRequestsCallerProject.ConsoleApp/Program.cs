using Newtonsoft.Json.Linq;
using QueuedRequestsCaller;
using QueuedRequestsCaller.Models;
using QueuedRequestsCaller.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QueuedRequestsCallerProject.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            // Create a list of QueuedRequestItem objects, each containing a request model, mapping list, and post-request actions list
            var callsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

            // Add a request to the namefake API with a mapping from the response body to a query parameter, and two post-request actions to log some information
            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
                "https://api.namefake.com/",
                new Dictionary<string, string>(),
                new Dictionary<string, string>(), null),

                MappingList = new List<QueuedRequestsCaller.Models.MapCouple>()
                {
                    new MapCouple()
                    {
                        From = new RequestValue()
                        {
                            FullName = "name",
                            Location = QueuedRequestsCaller.Enums.MappingValueLocation.Body
                        },
                        To = new RequestValue()
                        {
                            FullName = "name",
                            Location = QueuedRequestsCaller.Enums.MappingValueLocation.QueryParam
                        }
                    }
                },

                PostRequestActionsList = new List<Action<RequestModel, RequestModel>>()
                {
                    LogSomeInfo,
                    LogSomeInfo
                }
            });

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
               "https://api.nationalize.io/",
               new Dictionary<string, string>(),
               new Dictionary<string, string>(), null)
            });

            // Create a QueuedRequestsCallerService instance with the callsList as a parameter
            QueuedRequestsCallerService caller = new QueuedRequestsCallerService(new QueuedRequestsCallerSettings() { RequestsList = callsList });

            // Call MakeRequests method of QueuedRequestsCallerService to execute all requests sequentially
            var result = caller.MakeRequests();

            // Print the result status and response content or last exception message
            Console.WriteLine("Result Status: " + result.IsSuccessfully);
            if (result.IsSuccessfully)
                Console.WriteLine("Result: " + result.Response.Content);
            else
                Console.Write(result.LastException);

            Console.WriteLine("Finish");

            Console.ReadLine();
        }

        // A method to log some information about the current request
        public static void LogSomeInfo(RequestModel current, RequestModel next)
        {
            Console.WriteLine("");
            Console.WriteLine(JObject.Parse(current.RequestResponse.Content));
            Console.WriteLine("");
        }
    }
}

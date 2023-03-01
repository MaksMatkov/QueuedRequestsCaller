using QueuedRequestsCaller;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;

namespace QueuedRequestsCallerProject.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            var callsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem() { 
                model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
                "https://api.namefake.com/", 
                new Dictionary<string, string>(),
                new Dictionary<string, string>(), null),
                mappingList = new List<QueuedRequestsCaller.Models.MapCouple>()
                {
                    new MapCouple()
                    {
                        From = new RequestValue()
                        {
                            FullName = "name",
                            location = QueuedRequestsCaller.Enums.MappingValueLocation.Body
                        },
                        To = new RequestValue()
                        {
                            FullName = "name",
                            location = QueuedRequestsCaller.Enums.MappingValueLocation.QueryParam
                        }
                    }
                }
            });

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
               "https://api.nationalize.io/",
               new Dictionary<string, string>(),
               new Dictionary<string, string>(), null)
            });

            QueuedRequestsCallerService caller = new QueuedRequestsCallerService(callsList);

            var response = caller.MakeRequests();

            Console.WriteLine(response.Content);

            Console.WriteLine("Finish");

            Console.ReadLine();
        }
    }
}

﻿using QueuedRequestsCaller;
using QueuedRequestsCaller.Models;
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

            var callsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
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

            QueuedRequestsCallerService caller = new QueuedRequestsCallerService(new QueuedRequestsCallerSettings() { RequestsList = callsList });

            var result = caller.MakeRequests();

            result.RequestIteration.SelectMany(el => el.Logs)
                .ToList()
                .ForEach(el => Console.WriteLine("Log: " + el.DateTime.ToShortTimeString() + " " + el.Message));

            Console.WriteLine("Result Status: " + result.IsSuccessfully);
            if (result.IsSuccessfully)
                Console.WriteLine("Result: " + result.Response.Content);
            else
                Console.Write(result.LastException);

            Console.WriteLine("Finish");

            Console.ReadLine();
        }
    }
}
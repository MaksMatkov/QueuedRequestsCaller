using Newtonsoft.Json.Linq;
using QueuedRequestsCaller;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace RandomCatPhotoDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start!");

            var callsList = new List<QueuedRequestItem>();

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new RequestModel(RestSharp.Method.Get, "https://cataas.com/cat?json=true", null),
                CallsCount = 3, //set images count
                PostRequestActionsList = new List<Action<RequestModel, RequestModel>>
                {
                    Download,
                    SaveResponseData
                }
            });

            QueuedRequestsCallerService caller = new QueuedRequestsCallerService(new QueuedRequestsCallerSettings() { RequestsList = callsList, DropOnReExecuteError = true });

            var result = caller.MakeRequests();

            Console.WriteLine("Result Status: " + result.IsSuccessfully);
            if (result.IsSuccessfully)
                Console.WriteLine("Result: " + result.Response.Content);
            else
                Console.Write(result.LastException);

            Console.WriteLine("Finish!");
        }

        public static void Download(RequestModel curent, RequestModel next)
        {
            Task.Run(() => {
                string destinationFile = @"F:\myProj\TempFiles\"; //replace to your folder

                using (WebClient client = new WebClient())
                {
                    dynamic result = JObject.Parse(curent.RequestResponse.Content);
                    client.DownloadFile("https://cataas.com/" + result.url, destinationFile + result.file);
                }
            });
        }

        public static void SaveResponseData(RequestModel curent, RequestModel next)
        {
            File.AppendAllText(
                @"F:\myProj\TempFiles\Data.json", //replace to your folder
                curent.RequestResponse.Content, System.Text.Encoding.UTF8);
        }
    }
}

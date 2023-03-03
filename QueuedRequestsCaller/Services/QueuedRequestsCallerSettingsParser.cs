using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Infrastructure;
using QueuedRequestsCaller.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Services
{
    public class QueuedRequestsCallerSettingsParser : IQueuedRequestsCallerSettingsParser
    {
        public QueuedRequestsCallerSettings Parse(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
                throw new Exception("JSON is NULL");

            var JSON = JObject.Parse(json);
            var result = new QueuedRequestsCallerSettings();

            try
            {
                var requestsList = (JArray)JSON["RequestsList"];
                foreach (var request in requestsList)
                {
                    var model = request.SelectToken("Model");
                    var mappingList = JsonConvert.DeserializeObject<List<MapCouple>>(request.SelectToken("MappingList").ToString());

                    var method = (Method)model.SelectToken("Method").Value<int>();
                    var resource = model.SelectToken("Resourse").Value<string>();
                    var queryParams = model.SelectToken("QueryParameters").Value<JArray>().ToDictionary(k => ((JObject)k).Properties().First().Name, v => v.Values().First().Value<string>());
                    var headerValues = model.SelectToken("HeaderValues").Value<JArray>().ToDictionary(k => ((JObject)k).Properties().First().Name, v => v.Values().First().Value<string>());
                    var body = model.SelectToken("Body").ToString();

                    result.RequestsList.Add(new QueuedRequestItem()
                    {
                        MappingList = mappingList,
                        Model = new RequestModel(method, resource, queryParams, headerValues, body)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(ex.Message, ex);
            }

            return result;
        }
    }
}

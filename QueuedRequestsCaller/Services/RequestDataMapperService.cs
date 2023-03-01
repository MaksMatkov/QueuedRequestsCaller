using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Services
{
    public static class RequestDataMapperService
    {
        public static RequestModel MappToNext(this QueuedRequestItem currentRequest, RequestModel nextRequest)
        {

            foreach (var mappInfo in currentRequest.mappingList)
            {
                nextRequest.SetValueByLocation(mappInfo.To, currentRequest.GetValueByLocation(mappInfo.From));
            }

            return nextRequest;
        }

        public static object GetValueByLocation(this QueuedRequestItem currentRequest, RequestValue value)
        {
            switch (value.location)
            {
                case Enums.MappingValueLocation.Body:
                    return JObjectMapperService.ExtractField(JObject.Parse(currentRequest.model.RequestResponse.Content), value.FullName);
                case Enums.MappingValueLocation.Header:
                    return currentRequest.model.RequestResponse.Headers.FirstOrDefault(el => el.Name == value.FullName);
                case Enums.MappingValueLocation.QueryParam:
                    var queryDictionary = System.Web.HttpUtility.ParseQueryString(currentRequest.model.RequestResponse.ResponseUri.Query);
                    return queryDictionary.Get(value.FullName);
            }

            return null;
        }

        public static bool SetValueByLocation(this RequestModel nextRequest, RequestValue value, object newValue)
        {
            switch (value.location)
            {
                case Enums.MappingValueLocation.Body:
                    JObjectMapperService.CopyFieldValue(nextRequest.Body, value.FullName, newValue);
                    break;
                case Enums.MappingValueLocation.Header:
                    nextRequest.Headers[value.FullName] = (string)newValue;
                    break;
                case Enums.MappingValueLocation.QueryParam:
                    nextRequest.QueryParameters[value.FullName] = (string)newValue;
                    break;
            }

            return true;
        }
    }
}

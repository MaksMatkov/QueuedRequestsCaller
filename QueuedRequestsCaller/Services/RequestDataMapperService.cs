using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Services
{
    public static class RequestDataMapperService
    {
        /// <summary>
        /// Maps the data from the current request to the next request.
        /// </summary>
        /// <param name="currentRequest">The current request item.</param>
        /// <param name="nextRequest">The next request model to be mapped.</param>
        /// <returns>The next request model with the data mapped.</returns>
        public static RequestModel MappToNext(this QueuedRequestItem currentRequest, RequestModel nextRequest)
        {
            currentRequest.MappingList.AsParallel().ForAll((mappInfo) =>
            {
                try
                {
                    nextRequest.SetValueByLocation(mappInfo.To, currentRequest.GetValueByLocation(mappInfo.From));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during mapping MappInfo = [{JObject.FromObject(mappInfo)}]", ex);
                }
            });

            return nextRequest;
        }

        /// <summary>
        /// Gets the value from the current request based on the location specified in the request value.
        /// </summary>
        /// <param name="currentRequest">The current request item.</param>
        /// <param name="value">The request value that specifies the location to get the value from.</param>
        /// <returns>The value from the specified location in the current request.</returns>
        public static object GetValueByLocation(this QueuedRequestItem currentRequest, RequestValue value)
        {
            switch (value.Location)
            {
                case Enums.MappingValueLocation.Body:
                    return JObjectMapperService.ExtractField(JObject.Parse(currentRequest.Model.RequestResponse.Content), value.FullName);
                case Enums.MappingValueLocation.Header:
                    return currentRequest.Model.RequestResponse.Headers.FirstOrDefault(el => el.Name == value.FullName);
                case Enums.MappingValueLocation.QueryParam:
                    var queryDictionary = System.Web.HttpUtility.ParseQueryString(currentRequest.Model.RequestResponse.ResponseUri.Query);
                    return queryDictionary.Get(value.FullName);
            }

            return null;
        }

        /// <summary>
        /// Sets the value in the next request based on the location specified in the request value.
        /// </summary>
        /// <param name="nextRequest">The next request model.</param>
        /// <param name="value">The request value that specifies the location to set the value in.</param>
        /// <param name="newValue">The new value to set.</param>
        /// <returns>True if the value was set successfully, false otherwise.</returns>
        public static bool SetValueByLocation(this RequestModel nextRequest, RequestValue value, object newValue)
        {
            switch (value.Location)
            {
                case Enums.MappingValueLocation.Body:
                    JObjectMapperService.CopyFieldValue(nextRequest.RequestBody, value.FullName, newValue);
                    break;
                case Enums.MappingValueLocation.Header:
                    nextRequest.RestRequest.AddOrUpdateHeader(value.FullName, (string)newValue);
                    break;
                case Enums.MappingValueLocation.QueryParam:
                    nextRequest.RestRequest.AddOrUpdateParameter(value.FullName, (string)newValue);
                    break;
            }

            return true;
        }
    }
}

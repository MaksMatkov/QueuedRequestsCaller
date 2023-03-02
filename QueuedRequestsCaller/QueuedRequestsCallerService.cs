using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using RestSharp;
using System.Threading.Tasks;
using QueuedRequestsCaller.Services;
using System.Linq;

namespace QueuedRequestsCaller
{
    public class QueuedRequestsCallerService
    {
        private QueuedRequestsCallerSettings _settings { get; set; }

        public QueuedRequestsCallerService(QueuedRequestsCallerSettings settings)
        {
            this._settings = settings;
        }

        public RequestsCallerResult MakeRequests()
        {
            if (!_settings.RequestsList.Any())
                return null;

            var _requestIteration = new List<RequestIteration>();

            for(var i = 0; i < _settings.RequestsList.Count; i++)
            {
                var newIteration = new RequestIteration(_settings.RequestsList[i].model);
                try
                {
                    newIteration.AddLog(new Log($"Start execute request with endpoint = [{newIteration.RequestModel.RestRequest.Resource}]"));

                    _settings.RequestsList[i].model.MakeRequest();

                    if (i + 1 < _settings.RequestsList.Count)
                    {
                        newIteration.AddLog(new Log($"Start mapping values to next request, Values Count = [{_settings.RequestsList[i].mappingList.Count}]"));

                        _settings.RequestsList[i].MappToNext(_settings.RequestsList[i + 1].model);
                        
                        newIteration.AddLog(new Log($"End mapping values to next request"));
                    }
                    
                    newIteration.AddLog(new Log($"End execute request with endpoint = [{newIteration.RequestModel.RestRequest.Resource}]"));
                    
                    _requestIteration.Add(newIteration);
                }
                catch(Exception ex)
                {
                    newIteration.AddLog(new Log($"Error: Request endpoint = [{newIteration.RequestModel.RestRequest.Resource}], Message = [{ex.Message}]", Enums.RequestLogType.Error, ex));
                    return new RequestsCallerResult(false, _requestIteration, ex);
                }
            }

            return new RequestsCallerResult(true, _requestIteration);
        }
    }
}

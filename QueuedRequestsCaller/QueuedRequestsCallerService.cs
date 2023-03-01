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

            RestResponse responsePointer = new RestResponse();
            var _requestIteration = new List<RequestIteration>();

            foreach (var req in _settings.RequestsList)
            {
                var newIteration = new RequestIteration(req.model);
                try
                {
                    newIteration.AddLog(new Log($"Start execute request with endpoint = [{newIteration.RequestModel.Resource}]"));
                    req.model.MakeRequest();
                    if (_settings.RequestsList.IndexOf(req) != _settings.RequestsList.Count - 1)
                    {
                        newIteration.AddLog(new Log($"Start mapping values to next request, Values Count = [{req.mappingList.Count}]"));
                        req.MappToNext(_settings.RequestsList[_settings.RequestsList.IndexOf(req) + 1].model);
                        newIteration.AddLog(new Log($"End mapping values to next request"));
                    }

                    responsePointer = req.model.RequestResponse;
                    
                    newIteration.AddLog(new Log($"End execute request with endpoint = [{newIteration.RequestModel.Resource}]"));
                    _requestIteration.Add(newIteration);
                }
                catch(Exception ex)
                {
                    newIteration.AddLog(new Log($"Error: Request endpoint = [{newIteration.RequestModel.Resource}], Message = [{ex.Message}]", Enums.RequestLogType.Error, ex));
                    return new RequestsCallerResult(false, _requestIteration, ex);
                }
            }

            return new RequestsCallerResult(true, _requestIteration);
        }
    }
}

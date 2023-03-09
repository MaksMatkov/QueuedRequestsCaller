using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using RestSharp;
using System.Threading.Tasks;
using QueuedRequestsCaller.Services;
using System.Linq;
using QueuedRequestsCaller.Infrastructure;
using QueuedRequestsCaller.Exceptions;

namespace QueuedRequestsCaller
{
    public class QueuedRequestsCallerService
    {
        private QueuedRequestsCallerSettings _callerSettings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedRequestsCallerService"/> class.
        /// </summary>
        /// <param name="callerSettings"><see cref="QueuedRequestsCallerService"/> settings</param>
        public QueuedRequestsCallerService(QueuedRequestsCallerSettings callerSettings)
        {
            this._callerSettings = callerSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedRequestsCallerService"/> class. That take json string with <see cref="IQueuedRequestsCallerSettingsParser"/>.
        /// <para>Use for implement dynamic request caller settings editing</para>
        /// </summary>
        /// <param name="json"><see cref="QueuedRequestsCallerSettings"/> as JSON string</param>
        /// <param name="parce"><see cref="IQueuedRequestsCallerSettingsParser"/></param>
        /// <param name="needValidate">if need validate parsed json before parse, if false can improve execution speed</param>
        public QueuedRequestsCallerService(string json, IQueuedRequestsCallerSettingsParser parce, bool needValidate = true)
        {
            if (parce == null)
                throw new NullReferenceException($"{typeof(IQueuedRequestsCallerSettingsParser).FullName} is NULL");

            this._callerSettings = parce.Parse(json, needValidate);
        }

        /// <summary>
        /// Makes requests according to the queued request settings.
        /// </summary>
        /// <returns>
        /// A <see cref="RequestsCallerResult"/> object containing the result of the requests, 
        /// or null if <see cref="QueuedRequestsCallerSettings"/> is null or <see cref="QueuedRequestsCallerSettings.RequestsList"/> is empty or null.
        /// </returns>
        public RequestsCallerResult MakeRequests()
        {
            if (_callerSettings == null || !(_callerSettings.RequestsList != null && _callerSettings.RequestsList.Any()))
                return null;

            var _requestIteration = new List<RequestIteration>();

            for (var i = 0; i < _callerSettings.RequestsList.Count; i++)
            {
                var newIteration = new RequestIteration(_callerSettings.RequestsList[i].Model);
                try
                {
                    newIteration.AddLog(new Log($"Start execute request with endpoint = [{newIteration.RequestModel.RestRequest.Resource}]"));

                    _callerSettings.RequestsList[i].Model.MakeRequest();

                    if (_callerSettings.RequestsList[i].ExpectedStatusesList.Length > 0
                        && !_callerSettings.RequestsList[i].ExpectedStatusesList.Contains((int)_callerSettings.RequestsList[i].Model.RequestResponse.StatusCode))
                        throw new NotMatchStatusException("Not Match Status", (int)_callerSettings.RequestsList[i].Model.RequestResponse.StatusCode);


                    //execute post request actions
                    if (_callerSettings.RequestsList[i].CallsCount > 1)
                    {
                        newIteration.AddLog(new Log($"Start re-execute request with endpoint = [{newIteration.RequestModel.RestRequest.Resource}], CallsCount = [{_callerSettings.RequestsList[i].CallsCount}]"));

                        for (var j = 0; j < _callerSettings.RequestsList[i].CallsCount; j++)
                        {
                            try
                            {
                                CallPostRequestActions(_callerSettings.RequestsList[i], i + 1 < _callerSettings.RequestsList.Count ? _callerSettings.RequestsList[i + 1] : null);
                                _callerSettings.RequestsList[i].Model.MakeRequest();
                            }
                            catch (Exception ex)
                            {
                                newIteration.AddLog(new Log($"Error during re-execute request with endpoint = [{newIteration.RequestModel.RestRequest.Resource}], Time = [{j + 1}]", Enums.RequestLogType.Error, ex));
                                if (_callerSettings.DropOnReExecuteError)
                                {
                                    _requestIteration.Add(newIteration);
                                    throw new Exception("DropOnReExecuteError", ex);
                                }
                                else
                                    break;
                            }
                        }

                        newIteration.AddLog(new Log($"End re-execute request with endpoint = [{newIteration.RequestModel.RestRequest.Resource}]"));
                    }
                    else if (i + 1 < _callerSettings.RequestsList.Count)
                    {
                        CallPostRequestActions(_callerSettings.RequestsList[i], _callerSettings.RequestsList[i + 1]);

                        newIteration.AddLog(new Log($"Start mapping values to next request, Values Count = [{_callerSettings.RequestsList[i].MappingList.Count}]"));

                        _callerSettings.RequestsList[i].MappToNext(_callerSettings.RequestsList[i + 1].Model);

                        newIteration.AddLog(new Log($"End mapping values to next request"));
                    }

                    newIteration.AddLog(new Log($"End execute request with endpoint = [{newIteration.RequestModel.RestRequest.Resource}]"));

                    _requestIteration.Add(newIteration);
                }
                catch (Exception ex)
                {
                    newIteration.AddLog(new Log($"Error: Request endpoint = [{newIteration.RequestModel.RestRequest.Resource}], Message = [{ex.Message}]", Enums.RequestLogType.Error, ex));
                    return new RequestsCallerResult(false, _requestIteration, ex);
                }
            }

            return new RequestsCallerResult(true, _requestIteration);
        }

        public void CallPostRequestActions(QueuedRequestItem requestItem, QueuedRequestItem nextRequestItem)
        {
            for (var j = 0; j < requestItem.PostRequestActionsList.Count; j++)
                requestItem.PostRequestActionsList[j](requestItem.Model, nextRequestItem != null ? nextRequestItem.Model : null);
        }
    }
}

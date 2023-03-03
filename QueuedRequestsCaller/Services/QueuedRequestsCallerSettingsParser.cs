using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using QueuedRequestsCaller.Exceptions;
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
        /// <summary>
        /// Parses the JSON string and creates a <see cref="QueuedRequestsCallerSettings"/>  object.
        /// </summary>
        /// <param name="json">JSON string to parse</param>
        /// <param name="needValidate">>Whether the parsed object needs to be validated. Default is true.</param>
        /// <returns>
        /// A <see cref="QueuedRequestsCallerSettings"/>. 
        /// </returns>
        /// <exception cref="JsonException"> Thrown when the JSON string is not provided.</exception>
        /// <exception cref="JsonValidationException"> Thrown when the provided JSON string is not valid.</exception>
        /// <exception cref="InvalidCastException">Thrown when the provided JSON string cannot be deserialized into <see cref="QueuedRequestsCallerSettings"/> object.</exception>
        public QueuedRequestsCallerSettings Parse(string json, bool needValidate = true)
        {
            if (String.IsNullOrWhiteSpace(json))
                throw new JsonException("JSON is NULL");

            if (needValidate)
            {
                var validationResult = Validate(json);
                if (!validationResult.IsValid)
                    throw new JsonValidationException("Json Validation Error", validationResult.Errors);
            }

            var JSON = JObject.Parse(json);
            var result = new QueuedRequestsCallerSettings();

            try
            {
                var requestsList = (JArray)JSON["RequestsList"];
                foreach (var request in requestsList)
                {
                    var model = request.SelectToken("Model");
                    var callsCount = request.SelectToken("CallsCount") != null ? request.SelectToken("CallsCount").Value<int>() : 1;

                    var mappingList = request.SelectToken("MappingList") != null ? 
                        JsonConvert.DeserializeObject<List<MapCouple>>(request.SelectToken("MappingList")?.ToString())
                        : new List<MapCouple>();

                    var method = (Method)model.SelectToken("Method").Value<int>();
                    var resource = model.SelectToken("Resourse").Value<string>();
                    var queryParams = model.SelectToken("QueryParameters")?.Value<JArray>().ToDictionary(k => ((JObject)k).Properties().First().Name, v => v.Values().First().Value<string>());
                    var headerValues = model.SelectToken("HeaderValues")?.Value<JArray>().ToDictionary(k => ((JObject)k).Properties().First().Name, v => v.Values().First().Value<string>());
                    var body = model.SelectToken("Body")?.ToString();

                    result.RequestsList.Add(new QueuedRequestItem()
                    {
                        MappingList = mappingList,
                        Model = new RequestModel(method, resource, body, queryParams, headerValues),
                        CallsCount = callsCount > 0 ? callsCount : 1
                    });
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Validates the JSON string by RequestsCallerSettings schema.
        /// </summary>
        /// <param name="json">JSON string to validate</param>
        /// <returns>
        /// A <see cref="RequestsCallerSettingsValidationResult"/> object containing the validation result. 
        /// </returns>
        public RequestsCallerSettingsValidationResult Validate(string json)
        {
            string schemaJson = @"{
          ""$schema"": ""http://json-schema.org/draft-07/schema#"",
          ""type"": ""object"",
          ""properties"": {
            ""RequestsList"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""object"",
                ""properties"": {
                  ""Model"": {
                    ""type"": ""object"",
                    ""properties"": {
                      ""Method"": {
                        ""type"": ""integer""
                      },
                      ""Resourse"": {
                        ""type"": ""string""
                      },
                      ""QueryParameters"": {
                        ""type"": ""array"",
                        ""items"": {
                          ""type"": ""object"",
                          ""properties"": {
                            ""value"": {
                              ""type"": ""integer""
                            },
                            ""value1"": {
                              ""type"": ""integer""
                            }
                          }
                        }
                      },
                      ""HeaderValues"": {
                        ""type"": ""array"",
                        ""items"": {
                          ""type"": ""object"",
                          ""properties"": {
                            ""value"": {
                              ""type"": ""integer""
                            },
                            ""value1"": {
                              ""type"": ""integer""
                            }
                          }
                        }
                      },
                      ""Body"": {
                        ""type"": ""object""
                      }
                    },
                    ""required"": [
                      ""Method"",
                      ""Resourse""
                    ]
                  },
                  ""MappingList"": {
                    ""type"": ""array"",
                    ""items"": {
                      ""type"": ""object"",
                      ""properties"": {
                        ""From"": {
                          ""type"": ""object"",
                          ""properties"": {
                            ""Location"": {
                              ""type"": ""string""
                            },
                            ""FullName"": {
                              ""type"": ""string""
                            }
                          },
                          ""required"": [
                            ""Location"",
                            ""FullName""
                          ]
                        },
                        ""To"": {
                          ""type"": ""object"",
                          ""properties"": {
                            ""Location"": {
                              ""type"": ""string""
                            },
                            ""FullName"": {
                              ""type"": ""string""
                            }
                          },
                          ""required"": [
                            ""Location"",
                            ""FullName""
                          ]
                        }
                      },
                      ""required"": [
                        ""From"",
                        ""To""
                      ]
                    }
                  }
                },
                ""required"": [
                  ""Model""
                ]
              }
            }
          },
          ""CallsCount"": {
                ""type"": ""integer""
          },
          ""required"": [
            ""RequestsList""
          ]
        }";

            JSchema schema = JSchema.Parse(schemaJson);
            JObject jsonObject = JObject.Parse(json);

            IList<ValidationError> errors;
            bool isValid = jsonObject.IsValid(schema, out errors);

            return new RequestsCallerSettingsValidationResult(isValid, errors);
        }
    }
}

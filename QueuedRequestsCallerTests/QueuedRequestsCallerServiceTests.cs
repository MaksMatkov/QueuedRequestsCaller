using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using QueuedRequestsCaller;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Tests
{
    [TestClass()]
    public class QueuedRequestsCallerServiceTests
    {
        [TestMethod()]
        public void MakeRequests_WithEmptyRequestsList_ReturnsNull()
        {
            // Arrange
            var settings = new QueuedRequestsCallerSettings
            {
                RequestsList = new List<QueuedRequestItem>()
            };
            var service = new QueuedRequestsCallerService(settings);

            // Act
            var result = service.MakeRequests();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void MakeRequests_WithOneRequest_ReturnsSuccessfulResult()
        {
            // Arrange
            var requestsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

            requestsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
                "https://api.namefake.com/",
                new Dictionary<string, string>(),
                new Dictionary<string, string>(), null),
            });
            var settings = new QueuedRequestsCallerSettings
            {
                RequestsList = requestsList
            };
            var service = new QueuedRequestsCallerService(settings);

            // Act
            var result = service.MakeRequests();

            // Assert
            Assert.IsTrue(result.IsSuccessfully);
            Assert.AreEqual(1, result.RequestIteration.Count);
        }

        [TestMethod()]
        public void MakeRequests_WithOneRequest_WithSettingCustomParameters_ReturnsSuccessfulResult()
        {
            // Arrange
            var requestsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

            requestsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
                "https://api.namefake.com/",
                new Dictionary<string, string>() { { "test", "test" }, { "test2", "test2" } },
                new Dictionary<string, string>() { { "headerTest" , "value"} }, null),
            });
            var settings = new QueuedRequestsCallerSettings
            {
                RequestsList = requestsList
            };
            var service = new QueuedRequestsCallerService(settings);

            // Act
            var result = service.MakeRequests();

            // Assert
            Assert.IsTrue(result.IsSuccessfully);
            Assert.AreEqual(1, result.RequestIteration.Count);
            Assert.IsTrue(result.Response.Request.Parameters.TryFind("test") != null && (string)result.Response.Request.Parameters.TryFind("test").Value == "test");
            Assert.IsTrue(result.Response.Request.Parameters.TryFind("test2") != null && (string)result.Response.Request.Parameters.TryFind("test2").Value == "test2");
            Assert.IsTrue(result.Response.Request.Parameters.TryFind("headerTest") != null && (string)result.Response.Request.Parameters.TryFind("headerTest").Value == "value");
        }

        [TestMethod()]
        public void MakeRequests_WithSomeRequests()
        {
            // Arrange
            var callsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
                "https://api.namefake.com/",
                new Dictionary<string, string>(),
                new Dictionary<string, string>(), null),

                MappingList = new List<QueuedRequestsCaller.Models.MapCouple>()
                {
                    new MapCouple()
                    {
                        From = new RequestValue()
                        {
                            FullName = "name",
                            Location = QueuedRequestsCaller.Enums.MappingValueLocation.Body
                        },
                        To = new RequestValue()
                        {
                            FullName = "name",
                            Location = QueuedRequestsCaller.Enums.MappingValueLocation.QueryParam
                        }
                    }
                }
            });

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
               "https://api.nationalize.io/",
               new Dictionary<string, string>(),
               new Dictionary<string, string>(), null)
            });

            QueuedRequestsCallerService caller = new QueuedRequestsCallerService(new QueuedRequestsCallerSettings() { RequestsList = callsList });

            // Act
            var result = caller.MakeRequests();

            // Assert
            Assert.IsTrue(result.IsSuccessfully);
            Assert.AreEqual((JObject.Parse(result.RequestIteration[0].RequestModel.RequestResponse.Content) as dynamic).name, (JObject.Parse(result.Response.Content) as dynamic).name);
        }

        [TestMethod()]
        public void MakeRequests_WithSomeRequests_InncorrectMapping()
        {
            // Arrange
            var callsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
                "https://api.namefake.com/",
                new Dictionary<string, string>(),
                new Dictionary<string, string>(), null),

                MappingList = new List<QueuedRequestsCaller.Models.MapCouple>()
                {
                    new MapCouple()
                    {
                        From = new RequestValue()
                        {
                            FullName = "nameInncorrect",
                            Location = QueuedRequestsCaller.Enums.MappingValueLocation.Body
                        },
                        To = new RequestValue()
                        {
                            FullName = "name",
                            Location = QueuedRequestsCaller.Enums.MappingValueLocation.QueryParam
                        }
                    }
                }
            });

            callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
            {
                Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
               "https://api.nationalize.io/",
               new Dictionary<string, string>(),
               new Dictionary<string, string>(), null)
            });

            QueuedRequestsCallerService caller = new QueuedRequestsCallerService(new QueuedRequestsCallerSettings() { RequestsList = callsList });

            // Act
            var result = caller.MakeRequests();

            // Assert
            Assert.IsFalse(result.IsSuccessfully);
            Assert.IsNotNull(result.LastException);
        }

        [TestMethod()]
        public void MakeRequests_WithSomeRequests_NullInput()
        {
            // Arrange
            QueuedRequestsCallerSettings input = null;
            QueuedRequestsCallerService caller = new QueuedRequestsCallerService(input);

            // Act
            var result = caller.MakeRequests();

            // Assert
            Assert.IsNull(result);
        }
    }
}
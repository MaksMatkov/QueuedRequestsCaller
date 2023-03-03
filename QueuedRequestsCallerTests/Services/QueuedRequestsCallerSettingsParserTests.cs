using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueuedRequestsCaller.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Services.Tests
{
    [TestClass()]
    public class QueuedRequestsCallerSettingsParserTests
    {
        [TestMethod()]
        public void ParseTest_AllDataCorrect1()
        {
            // Arrange
            var json = File.ReadAllText("TestJSONData.json");
            var servise = new QueuedRequestsCallerSettingsParser();

            // Act
            var result = servise.Parse(json);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.RequestsList);
            Assert.AreEqual(result.RequestsList.Count, 2);
            //check mapping list
            Assert.AreEqual(result.RequestsList[0].MappingList[0].From.FullName, "value1");
            Assert.AreEqual(result.RequestsList[0].MappingList[0].To.FullName, "value2");
            Assert.AreEqual(result.RequestsList[1].MappingList[0].From.FullName, "value1");
            Assert.AreEqual(result.RequestsList[1].MappingList[0].To.FullName, "value2");
        }

        [TestMethod()]
        public void ParseTest_EmptyJson()
        {
            // Arrange
            var json = "";
            var servise = new QueuedRequestsCallerSettingsParser();

            // Act
            try
            {
                var result = servise.Parse(json);
            }
            catch(Exception ex)
            {
                // Assert
                Assert.IsNotNull(ex, ex.Message);
                Assert.AreEqual(ex.Message, "JSON is NULL");
                return;
            }

            Assert.IsTrue(false);
        }

        [TestMethod()]
        public void ParseTest_IncorrectJson()
        {
            // Arrange
            var json = "{'data' : 'Something'}";
            var servise = new QueuedRequestsCallerSettingsParser();

            // Act
            try
            {
                var result = servise.Parse(json);
            }
            catch (InvalidCastException ex)
            {
                // Assert
                Assert.IsNotNull(ex, ex.Message);
                return;
            }

            Assert.IsTrue(false);
        }
    }
}
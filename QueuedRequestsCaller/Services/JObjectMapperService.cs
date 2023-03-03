using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Services
{
    public static class JObjectMapperService
    {
        /// <summary>
        /// Set new value to object property.
        /// <para>Examples: </para>
        /// <para>[ <see cref="fieldName"/> = 'valueOne', <see cref="newValue"/> = 1 ]  -> change value of first object property with name 'valueOne' to 1</para> 
        /// <para>[ <see cref="fieldName"/> = 'valueOne.valueTwo', <see cref="newValue"/> = 1 ]  -> change value of first inner object with name 'valueOne', set value 1 to property with name 'valueTwo'</para> 
        /// </summary>
        /// <param name="jObject">Object to new value integrating</param>
        /// <param name="fieldName">Path to value, can contain '.' to point the inner object property</param>
        /// <param name="newValue">New value of object property</param>
        public static void CopyFieldValue(JObject jObject, string fieldName, object newValue)
        {
            if (fieldName.Contains("."))
            {
                string[] parentChildFields = fieldName.Split('.');

                JToken jChild = jObject[parentChildFields[0]];

                for (int i = 1; i < parentChildFields.Length - 1; i++)
                {
                    jChild = jChild[parentChildFields[i]];
                }

                jChild[parentChildFields[parentChildFields.Length - 1]] = JToken.FromObject(newValue);
            }
            else
            {
                jObject[fieldName] = JToken.FromObject(newValue);
            }
        }

        /// <summary>
        /// Extract value from <see cref="JObject"/> by <see cref="fieldName"/> 
        /// <para><see cref="fieldName"/> can contain '.' that mean inner object property location</para>
        /// </summary>
        /// <param name="jsonObject">The object from which the value will extracted</param>
        /// <param name="fieldName">Value name or path(contains '.' that mean inner object property location)</param>
        /// <returns></returns>
        public static object ExtractField(JObject jsonObject, string fieldName)
        {
            string[] fieldNames = fieldName.Split('.');
            JToken currentToken = jsonObject;
            foreach (string name in fieldNames)
            {
                currentToken = currentToken[name];
            }
            return currentToken.ToString();
        }

        /// <summary>
        /// Map every value from one <see cref="JObject"/> to another
        /// </summary>
        /// <param name="copyToObject">Object that takes new values</param>
        /// <param name="copyFromObject">Object that gives new values</param>
        /// <param name="couples">List of <see cref="MapCouple"/> with mapping info</param>
        public static void MapValues(this JObject copyToObject, JObject copyFromObject, List<MapCouple> couples)
        {
            foreach (var c in couples)
            {
                CopyFieldValue(copyToObject, c.To.FullName, ExtractField(copyFromObject, c.From.FullName));
            }
        }
    }
}

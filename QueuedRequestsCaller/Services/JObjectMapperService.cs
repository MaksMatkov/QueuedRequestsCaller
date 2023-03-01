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
		public static JObject MapValues(JObject one, JObject two, List<MapCouple> couples)
		{
			foreach (var c in couples)
			{
                CopyFieldValue(two, c.To.FullName, ExtractField(one, c.From.FullName));
			}
			return two;
		}

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
    }
}

using Newtonsoft.Json.Linq;
using QueuedRequestsCaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Infrastructure
{
    public interface IQueuedRequestsCallerSettingsParser
    {
        /// <summary>
        /// Parses the JSON string and creates a <see cref="QueuedRequestsCallerSettings"/>  object.
        /// </summary>
        /// <param name="json">JSON string to parse</param>
        /// <param name="needValidate">>Whether the parsed object needs to be validated. Default is true.</param>
        /// <returns>
        /// A <see cref="QueuedRequestsCallerSettings"/>. 
        /// </returns>
        public QueuedRequestsCallerSettings Parse(string json, bool needValidate = true);

        /// <summary>
        /// Validates the JSON string by RequestsCallerSettings schema.
        /// </summary>
        /// <param name="json">JSON string to validate</param>
        /// <returns>
        /// A <see cref="RequestsCallerSettingsValidationResult"/> object containing the validation result. 
        /// </returns>
        public RequestsCallerSettingsValidationResult Validate(string json);
    }
}

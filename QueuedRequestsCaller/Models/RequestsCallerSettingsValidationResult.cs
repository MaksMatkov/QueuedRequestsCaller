using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Models
{
    public class RequestsCallerSettingsValidationResult
    {
        public RequestsCallerSettingsValidationResult(bool isValid, IList<ValidationError> errors)
        {
            this.IsValid = isValid;
            this.Errors = errors;
        }

        public bool IsValid { get; private set; }
        public IList<ValidationError> Errors { get; private set; }
    }
}

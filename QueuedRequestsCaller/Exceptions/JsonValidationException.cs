using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QueuedRequestsCaller.Exceptions
{
    public class JsonValidationException : Exception
    {
        public IList<ValidationError> validationErrors { get; private set; }
        
        public JsonValidationException()
        {
        }

        public JsonValidationException(string message, IList<ValidationError> validationErrors) : base(message)
        {
            this.validationErrors = validationErrors;
        }
    }
}

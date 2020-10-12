using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace TCStudentRecordManagement.Controllers.Exceptions
{
    public class APIException : Exception
    {
        public APIException() { }
        public APIException(string message) : base(message) { }
        public List<Exception> Exceptions { get; set; } = new List<Exception>();
        public APIException(string message, Exception inner) : base(message, inner) { }
        protected APIException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public override string ToString()
        {
            return $"Exception list with {Exceptions.Count()} items";
        }

        public void AddException(Exception exceptionItem)
        {
            Exceptions.Add(exceptionItem);
        }

        public void AddExMessage(string message)
        {
            Exception ex = new Exception(message);
            AddException(ex);
        }

        public bool HasExceptions { get => (Exceptions.Count() > 0); }
        
    }
}

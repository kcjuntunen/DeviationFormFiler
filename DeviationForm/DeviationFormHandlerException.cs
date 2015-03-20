using System;
using System.Collections.Generic;
using System.Text;

namespace DeviationForm
{
    [Serializable]
    public class DeviationFormHandlerException : Exception
    {
        public DeviationFormHandlerException() { }
        public DeviationFormHandlerException(string message) : base(message) { }
        public DeviationFormHandlerException(string message, Exception inner) : base(message, inner) { }
        protected DeviationFormHandlerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

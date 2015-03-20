using System;
using System.Collections.Generic;
using System.Text;

namespace DeviationForm
{
    [Serializable]
    public class DeviationDataHandlerException  : Exception
    {
        public DeviationDataHandlerException() { }
        public DeviationDataHandlerException(string message) : base(message) { }
        public DeviationDataHandlerException(string message, Exception inner) : base(message, inner) { }
        protected DeviationDataHandlerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

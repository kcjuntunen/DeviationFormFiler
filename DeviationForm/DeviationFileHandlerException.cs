using System;
using System.Collections.Generic;
using System.Text;

namespace DeviationForm
{
    [Serializable]
    public class DeviationFileHandlerException : Exception
    {
        public DeviationFileHandlerException() { }
        public DeviationFileHandlerException(string message) : base(message) { }
        public DeviationFileHandlerException(string message, Exception inner) : base(message, inner) { }
        protected DeviationFileHandlerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

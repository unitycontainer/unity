using System;
using System.Runtime.Serialization;

namespace Unity.TestSupport
{
    [DataContract]
    public class AssertFailedException : Exception
    {
        public AssertFailedException()
        {
        }

        public AssertFailedException(string message) : base(message)
        {
        }

        public AssertFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
#if !DNXCORE50
        protected AssertFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}
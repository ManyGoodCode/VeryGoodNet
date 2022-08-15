using System;
using System.Runtime.Serialization;

namespace Fclp
{
    public class InvalidOptionNameException : Exception
    {
        public InvalidOptionNameException()
        {
        }

        public InvalidOptionNameException(string message)
            : base(message)
        {
        }

        public InvalidOptionNameException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidOptionNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace EasyHttp.Infrastructure
{
    [Serializable]
    public class ConfigurationException : Exception
    {
        public ConfigurationException() { }

        public ConfigurationException(string message)
            : base(message) { }

        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException) { }

        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
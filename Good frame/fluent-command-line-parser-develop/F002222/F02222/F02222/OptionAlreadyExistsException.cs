using System;
using System.Runtime.Serialization;

namespace Fclp
{
    [Serializable]
    public class OptionAlreadyExistsException : Exception
    {
        public OptionAlreadyExistsException()
        { }

        public OptionAlreadyExistsException(string optionName)
            : base(optionName)
        { }


        public OptionAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public OptionAlreadyExistsException(string optionName, Exception innerException)
            : base(optionName, innerException)
        { }
    }
}

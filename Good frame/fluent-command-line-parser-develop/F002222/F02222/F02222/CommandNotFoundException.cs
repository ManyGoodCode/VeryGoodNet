using System;
using System.Runtime.Serialization;

namespace Fclp
{
    [Serializable]
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException() { }
        public CommandNotFoundException(string commandName) : base("Expected command " + commandName + " was not found in the parser.") { }
        public CommandNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
		
        public CommandNotFoundException(string optionName, Exception innerException)
            : base(optionName, innerException) { }
    }
}
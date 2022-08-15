using System;
using System.Runtime.Serialization;

namespace Fclp
{
    [Serializable]
    public class CommandAlreadyExistsException : Exception
    {
        public CommandAlreadyExistsException() { }
        public CommandAlreadyExistsException(string commandName) 
            : base(commandName) 
        { }
		
        public CommandAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { }
		
        public CommandAlreadyExistsException(string commandName, Exception innerException)
            : base(commandName, innerException)
        { }
    }
}
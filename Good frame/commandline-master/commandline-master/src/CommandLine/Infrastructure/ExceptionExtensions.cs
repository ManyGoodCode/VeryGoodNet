using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Infrastructure
{
    static class ExceptionExtensions
    {
        public static void RethrowWhenAbsentIn(this Exception exception, IEnumerable<Type> validExceptions)
        {
            if (!validExceptions.Contains(exception.GetType()))
            {
                throw exception;
            }
        }
    }
}

using System;
using System.Linq;

namespace WebApiContrib.Testing
{
    public class AssertionException : Exception
    {
        public AssertionException(string message)
            : base(message)
        {
        }

        public override string StackTrace
        {
            get
            {
                string Namespace = GetType().Namespace;
                string[] stacktracestring = SplitTheStackTraceByEachNewLine()
                    .Where(s => !s.TrimStart(' ').StartsWith("at " + Namespace))
                    .ToArray();
                return JoinArrayWithNewLineCharacters(stacktracestring);
            }
        }

        private string[] SplitTheStackTraceByEachNewLine()
        {
            return base.StackTrace.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string JoinArrayWithNewLineCharacters(string[] stacktracestring)
        {
            return string.Join(Environment.NewLine, stacktracestring);
        }
    }
}
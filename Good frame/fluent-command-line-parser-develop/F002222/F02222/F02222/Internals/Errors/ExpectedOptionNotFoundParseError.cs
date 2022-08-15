using System;

namespace Fclp.Internals.Errors
{
    public class ExpectedOptionNotFoundParseError : CommandLineParserErrorBase
    {
        public ExpectedOptionNotFoundParseError(ICommandLineOption cmdOption)
            : base(cmdOption)
        {
        }
    }
}

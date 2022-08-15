using System;
using Fclp.Internals.Parsing;

namespace Fclp.Internals.Errors
{
    public class OptionSyntaxParseError : CommandLineParserErrorBase
    {
        public ParsedOption ParsedOption { get; private set; }

        public OptionSyntaxParseError(ICommandLineOption cmdOption, ParsedOption parsedOption)
            : base(cmdOption)
        {
            ParsedOption = parsedOption;
        }
    }

    public class UnexpectedValueParseError : CommandLineParserErrorBase
    {
        public UnexpectedValueParseError(ICommandLineOption cmdOption)
            : base(cmdOption)
        {
        }
    }
}

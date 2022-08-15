using System;

namespace Fclp.Internals.Parsing.OptionParsers
{
    public class NullableCommandLineOptionParser<TNullableType> : ICommandLineOptionParser<TNullableType?> where TNullableType : struct
    {
        private readonly ICommandLineOptionParserFactory _parserFactory;
        public NullableCommandLineOptionParser(ICommandLineOptionParserFactory parserFactory)
        {
            _parserFactory = parserFactory;
        }

        public TNullableType? Parse(ParsedOption parsedOption)
        {
            ICommandLineOptionParser<TNullableType> parser = _parserFactory.CreateParser<TNullableType>();
            if (parser.CanParse(parsedOption) == false) 
                return null;
            return parser.Parse(parsedOption);
        }

        public bool CanParse(ParsedOption parsedOption)
        {
            return true;
        }
    }
}
namespace Fclp.Internals.Parsing.OptionParsers
{
    public class NullableBoolCommandLineOptionParser : ICommandLineOptionParser<bool?>
    {
        private readonly ICommandLineOptionParserFactory _parserFactory;
        public NullableBoolCommandLineOptionParser(ICommandLineOptionParserFactory parserFactory)
        {
            _parserFactory = parserFactory;
        }

        public bool? Parse(ParsedOption parsedOption)
        {
            ICommandLineOptionParser<bool> parser = _parserFactory.CreateParser<bool>();
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
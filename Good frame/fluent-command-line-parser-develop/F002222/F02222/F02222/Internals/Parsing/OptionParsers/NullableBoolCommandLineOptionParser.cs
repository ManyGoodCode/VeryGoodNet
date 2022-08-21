namespace Fclp.Internals.Parsing.OptionParsers
{
    /// <summary>
    /// ²¼¶û¿É¿Õ½âÎöÆ÷
    /// </summary>
    public class NullableBoolCommandLineOptionParser : ICommandLineOptionParser<bool?>
    {
        private readonly ICommandLineOptionParserFactory parserFactory;
        public NullableBoolCommandLineOptionParser(ICommandLineOptionParserFactory parserFactory)
        {
            this.parserFactory = parserFactory;
        }

        public bool? Parse(ParsedOption parsedOption)
        {
            ICommandLineOptionParser<bool> parser = parserFactory.CreateParser<bool>();
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
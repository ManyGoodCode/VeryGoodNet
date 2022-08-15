using System.Collections.Generic;
using System.Linq;

namespace Fclp.Internals.Parsing.OptionParsers
{
    public class ListCommandLineOptionParser<T> : ICommandLineOptionParser<List<T>>
    {
        private readonly ICommandLineOptionParserFactory _parserFactory;
        public ListCommandLineOptionParser(ICommandLineOptionParserFactory parserFactory)
        {
            _parserFactory = parserFactory;
        }

        public List<T> Parse(ParsedOption parsedOption)
        {
            ICommandLineOptionParser<T> parser = _parserFactory.CreateParser<T>();

            return parsedOption.Values.Select(value =>
            {
                ParsedOption clone = parsedOption.Clone();
                clone.Value = value;
                return parser.Parse(clone);
            }).ToList();
        }

        public bool CanParse(ParsedOption parsedOption)
        {
            if (parsedOption == null) 
                return false;
            if (parsedOption.HasValue == false) 
                return false;

            ICommandLineOptionParser<T> parser = _parserFactory.CreateParser<T>();
            return parsedOption.Values.All(
           value =>
            {
                ParsedOption clone = parsedOption.Clone();
                clone.Value = value;
                clone.Values = new[] { value };
                clone.AdditionalValues = new string[0];
                return parser.CanParse(clone);
            });
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Fclp.Internals.Parsing.OptionParsers
{
    /// <summary>
    /// ¼¯ºÏ½âÎöÆ÷
    /// </summary>
    public class ListCommandLineOptionParser<T> : ICommandLineOptionParser<List<T>>
    {
        private readonly ICommandLineOptionParserFactory parserFactory;
        public ListCommandLineOptionParser(ICommandLineOptionParserFactory parserFactory)
        {
            this.parserFactory = parserFactory;
        }

        public List<T> Parse(ParsedOption parsedOption)
        {
            ICommandLineOptionParser<T> parser = parserFactory.CreateParser<T>();
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

            ICommandLineOptionParser<T> parser = parserFactory.CreateParser<T>();
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

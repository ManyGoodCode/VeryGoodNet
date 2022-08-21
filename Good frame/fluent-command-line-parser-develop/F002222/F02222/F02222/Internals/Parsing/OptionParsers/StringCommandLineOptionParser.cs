using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing.OptionParsers
{
	/// <summary>
	/// ×Ö·û´®½âÎöÆ÷
	/// </summary>
	public class StringCommandLineOptionParser : ICommandLineOptionParser<string>
	{
		public string Parse(ParsedOption parsedOption)
		{
			if (parsedOption.Value == null)
				return null;
			return parsedOption.Value.ContainsWhitespace()
				? parsedOption.Value.RemoveAnyWrappingDoubleQuotes()
				: parsedOption.Value;
		}

		public bool CanParse(ParsedOption parsedOption)
		{
            if (parsedOption.Value.IsNullOrWhiteSpace()) 
				return true;
            if (parsedOption.HasValue == false) 
				return true;
            string value = (parsedOption.Value??"").Trim();
			System.Collections.Generic.IEnumerable<string> items = value.SplitOnWhitespace();
			return items.Count() == 1;
		}
	}
}
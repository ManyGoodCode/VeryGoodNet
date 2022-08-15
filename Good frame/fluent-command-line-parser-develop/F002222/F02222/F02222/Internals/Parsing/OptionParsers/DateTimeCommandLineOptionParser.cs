using System;
using System.Globalization;

namespace Fclp.Internals.Parsing.OptionParsers
{
	public class DateTimeCommandLineOptionParser : ICommandLineOptionParser<DateTime>
	{
		public DateTime Parse(ParsedOption parsedOption)
		{
            return DateTime.Parse(TrimAnyUnwantedCharacters(parsedOption.Value), CultureInfo.CurrentCulture);
		}

		public bool CanParse(ParsedOption parsedOption)
		{
			DateTime dtOut;
            return DateTime.TryParse(TrimAnyUnwantedCharacters(parsedOption.Value), out dtOut);
		}

	    private static string TrimAnyUnwantedCharacters(string value)
	    {
	        return (value ?? string.Empty).Trim('"');
	    }
	}
}
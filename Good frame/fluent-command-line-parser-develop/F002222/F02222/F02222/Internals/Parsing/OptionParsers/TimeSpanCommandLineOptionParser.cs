using System;
using System.Globalization;

namespace Fclp.Internals.Parsing.OptionParsers
{
	/// <summary>
	/// TimeSpan ½âÎöÆ÷
	/// </summary>
	public class TimeSpanCommandLineOptionParser : ICommandLineOptionParser<TimeSpan>
	{
		public TimeSpan Parse(ParsedOption parsedOption)
		{
            return TimeSpan.Parse(TrimAnyUnwantedCharacters(parsedOption.Value));
		}

		public bool CanParse(ParsedOption parsedOption)
		{
			TimeSpan dtOut;
            return TimeSpan.TryParse(TrimAnyUnwantedCharacters(parsedOption.Value), out dtOut);
		}

	    private static string TrimAnyUnwantedCharacters(string value)
	    {
	        return (value ?? string.Empty).Trim('"');
	    }
	}
}
using System.Globalization;

namespace Fclp.Internals.Parsing.OptionParsers
{
	/// <summary>
	/// Int32 ½âÎöÆ÷ int
	/// </summary>
	public class Int32CommandLineOptionParser : ICommandLineOptionParser<int>
	{
		public int Parse(ParsedOption parsedOption)
		{
			return int.Parse(parsedOption.Value, CultureInfo.CurrentCulture);
		}

		public bool CanParse(ParsedOption parsedOption)
		{
			int result;
			return int.TryParse(parsedOption.Value, out result);
		}
	}
}
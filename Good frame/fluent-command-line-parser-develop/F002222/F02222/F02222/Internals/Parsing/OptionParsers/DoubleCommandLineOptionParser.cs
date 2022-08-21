using System.Globalization;

namespace Fclp.Internals.Parsing.OptionParsers
{
	/// <summary>
	/// 双精度解析器通过 double.TryParse 来实现
	/// </summary>
	public class DoubleCommandLineOptionParser : ICommandLineOptionParser<double>
	{
		public double Parse(ParsedOption parsedOption)
		{
			return double.Parse(parsedOption.Value, CultureInfo.InvariantCulture);
		}

		public bool CanParse(ParsedOption parsedOption)
		{
			double result;
            return double.TryParse(parsedOption.Value, System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out result);
		}
	}
}
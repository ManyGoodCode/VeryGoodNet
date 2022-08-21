using System.Globalization;

namespace Fclp.Internals.Parsing.OptionParsers
{
	/// <summary>
	/// ˫���Ƚ�����ͨ�� double.TryParse ��ʵ��
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
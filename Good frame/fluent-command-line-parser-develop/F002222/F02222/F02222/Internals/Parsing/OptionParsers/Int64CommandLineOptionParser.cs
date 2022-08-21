using System.Globalization;

namespace Fclp.Internals.Parsing.OptionParsers
{
    /// <summary>
    /// Int64 解析器 long
    /// </summary>
    public class Int64CommandLineOptionParser : ICommandLineOptionParser<long>
    {
        public long Parse(ParsedOption parsedOption)
        {
            return long.Parse(parsedOption.Value, CultureInfo.CurrentCulture);
        }

        public bool CanParse(ParsedOption parsedOption)
        {
            long result;
            return long.TryParse(parsedOption.Value, out result);
        }
    }
}

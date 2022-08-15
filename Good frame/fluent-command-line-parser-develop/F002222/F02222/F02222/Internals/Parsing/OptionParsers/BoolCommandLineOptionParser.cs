using System;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing.OptionParsers
{
    public class BoolCommandLineOptionParser : ICommandLineOptionParser<bool>
    {
        private static readonly string[] recognisedFalseArgs = new[] { "off", "0" };
        private static readonly string[] recognisedTrueArgs = new[] { "on", "1" };
        public bool Parse(ParsedOption parsedOption)
        {
            if (parsedOption.Value.IsNullOrWhiteSpace())
            {
                return parsedOption.HasSuffix == false || parsedOption.Suffix != "-";
            }

            bool result;
            TryParse(parsedOption, out result);
            return result;
        }

        public bool CanParse(ParsedOption parsedOption)
        {
            bool result;
            return TryParse(parsedOption, out result);
        }

        private bool TryParse(ParsedOption parsedOption, out bool result)
        {
            if (parsedOption.Value.IsNullOrWhiteSpace())
            {
                result = parsedOption.HasSuffix == false || parsedOption.Suffix != "-";
                return true;
            }

            if (recognisedTrueArgs.Contains(parsedOption.Value, StringComparer.OrdinalIgnoreCase))
            {
                result = true;
                return true;
            }

            if (recognisedFalseArgs.Contains(parsedOption.Value, StringComparer.OrdinalIgnoreCase))
            {
                result = false;
                return true;
            }

            return bool.TryParse(parsedOption.Value, out result);
        }
    }
}
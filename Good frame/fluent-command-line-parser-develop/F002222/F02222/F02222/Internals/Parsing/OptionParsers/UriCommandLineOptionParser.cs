using System;

namespace Fclp.Internals.Parsing.OptionParsers
{
    public class UriCommandLineOptionParser : ICommandLineOptionParser<System.Uri>
    {
        public System.Uri Parse(ParsedOption parsedOption)
        {
            return new System.Uri(uriString: parsedOption.Value);
        }

        public bool CanParse(ParsedOption parsedOption)
        {
            try
            {
                new System.Uri(uriString: parsedOption.Value);
                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }
    }
}
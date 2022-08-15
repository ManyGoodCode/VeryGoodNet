using System;

namespace Fclp.Internals.Parsing.OptionParsers
{
	public class UriCommandLineOptionParser : ICommandLineOptionParser<Uri>
	{
        public Uri Parse(ParsedOption parsedOption)
		{
		    return new Uri(parsedOption.Value);
		}

		public bool CanParse(ParsedOption parsedOption)
		{
		    try
		    {
		        new Uri(parsedOption.Value);
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
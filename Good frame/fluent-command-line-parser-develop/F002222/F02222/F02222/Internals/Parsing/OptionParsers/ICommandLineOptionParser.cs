namespace Fclp.Internals.Parsing.OptionParsers
{
	public interface ICommandLineOptionParser<T>
	{
		T Parse(ParsedOption parsedOption);

		bool CanParse(ParsedOption parsedOption);
	}
}

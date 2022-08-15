using Fclp.Internals.Parsing.OptionParsers;

namespace Fclp.Internals.Parsing
{
	public interface ICommandLineOptionParserFactory
	{
		ICommandLineOptionParser<T> CreateParser<T>();
	}
}
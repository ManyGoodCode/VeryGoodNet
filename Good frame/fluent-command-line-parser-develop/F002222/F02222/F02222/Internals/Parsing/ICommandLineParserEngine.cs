namespace Fclp.Internals.Parsing
{
	public interface ICommandLineParserEngine
	{
	    ParserEngineResult Parse(string[] args, bool parseCommands);
	}
}

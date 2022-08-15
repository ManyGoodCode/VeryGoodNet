using System;
using Fclp.Internals.Parsing;
using Fclp.Internals.Parsing.OptionParsers;

namespace Fclp.Internals
{
	public class CommandLineOptionFactory : ICommandLineOptionFactory
	{

		ICommandLineOptionParserFactory _parserFactory;
		public ICommandLineOptionParserFactory ParserFactory
		{
			get { return _parserFactory ?? (_parserFactory = new CommandLineOptionParserFactory()); }
			set { _parserFactory = value; }
		}

		public ICommandLineOptionResult<T> CreateOption<T>(string shortName, string longName)
		{
			return new CommandLineOption<T>(shortName, longName, this.ParserFactory.CreateParser<T>());
		}

		public IHelpCommandLineOptionResult CreateHelpOption(string[] helpArgs)
		{
			return new HelpCommandLineOption(helpArgs);
		}
	}
}

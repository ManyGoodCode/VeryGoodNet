using System.Collections.Generic;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing
{
	public class ParserEngineResult
	{
	    public ParserEngineResult(IEnumerable<ParsedOption> parsedOptions, IEnumerable<string> additionalValues, string command)
		{
			ParsedOptions = parsedOptions ?? new List<ParsedOption>();
			AdditionalValues = additionalValues ?? new List<string>();
		    Command = command;
		}

        public string Command { get; private set; }
        public bool HasCommand 
		{ 
			get { return Command.IsNullOrWhiteSpace() == false; }
		}
		public IEnumerable<ParsedOption> ParsedOptions { get; private set; }
		public IEnumerable<string> AdditionalValues { get; private set; }
	}
}
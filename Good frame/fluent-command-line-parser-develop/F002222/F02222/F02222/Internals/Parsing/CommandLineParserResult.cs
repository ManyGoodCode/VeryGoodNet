using System.Collections.Generic;
using System.Linq;

namespace Fclp.Internals.Parsing
{
	public class CommandLineParserResult : ICommandLineParserResult
	{
		public CommandLineParserResult()
		{
			this.Errors = new List<ICommandLineParserError>();
			this.AdditionalOptionsFound = new List<KeyValuePair<string,string>>();
			this.UnMatchedOptions = new List<ICommandLineOption>();
		}

		public bool HasErrors
		{
			get { return this.Errors.Any(); }
		}

		internal IList<ICommandLineParserError> Errors { get; set; }
		IEnumerable<ICommandLineParserError> ICommandLineParserResult.Errors
		{
			get { return this.Errors; }
		}

		IEnumerable<KeyValuePair<string, string>> ICommandLineParserResult.AdditionalOptionsFound
		{
			get { return this.AdditionalOptionsFound; }
		}

		public IList<KeyValuePair<string, string>> AdditionalOptionsFound { get; set; }
		IEnumerable<ICommandLineOption> ICommandLineParserResult.UnMatchedOptions
		{
			get { return this.UnMatchedOptions; }
		}

	    public ParserEngineResult RawResult { get; set; }
		public IList<ICommandLineOption> UnMatchedOptions { get; set; }
		public bool HelpCalled { get; set; }
		public bool EmptyArgs { get; set; }
		public string ErrorText { get; set; }
	}
}
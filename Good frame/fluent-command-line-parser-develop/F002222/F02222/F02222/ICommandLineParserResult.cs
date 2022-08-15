using System.Collections.Generic;
using Fclp.Internals;
using Fclp.Internals.Parsing;

namespace Fclp
{
	public interface ICommandLineParserResult
	{
		bool HasErrors { get; }
		bool HelpCalled { get; }
		bool EmptyArgs { get; }
		string ErrorText { get; }
		IEnumerable<ICommandLineParserError> Errors { get; }
		IEnumerable<KeyValuePair<string, string>> AdditionalOptionsFound { get; }
		IEnumerable<ICommandLineOption> UnMatchedOptions { get; }
        ParserEngineResult RawResult { get; set; }
    }
}

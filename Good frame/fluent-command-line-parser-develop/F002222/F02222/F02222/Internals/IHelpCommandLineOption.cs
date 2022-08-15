using System;
using System.Collections.Generic;
using Fclp.Internals.Parsing;

namespace Fclp.Internals
{
	public interface IHelpCommandLineOption
	{
		bool ShouldShowHelp(IEnumerable<ParsedOption> parsedOptions, StringComparison comparisonType);
		void ShowHelp(IEnumerable<ICommandLineOption> options);
	}
}
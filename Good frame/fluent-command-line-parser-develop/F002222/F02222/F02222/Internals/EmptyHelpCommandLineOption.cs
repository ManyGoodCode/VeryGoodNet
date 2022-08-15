using System;
using System.Collections.Generic;
using Fclp.Internals.Parsing;

namespace Fclp.Internals
{
	public class EmptyHelpCommandLineOption : IHelpCommandLineOption
	{
		public bool ShouldShowHelp(IEnumerable<ParsedOption> commandLineArgs, StringComparison comparisonType)
		{
			return false;
		}

		public void ShowHelp(IEnumerable<ICommandLineOption> options)
		{
			throw new NotSupportedException();
		}
	}
}
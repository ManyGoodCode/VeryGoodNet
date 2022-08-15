using System.Collections.Generic;

namespace Fclp
{
	public interface ICommandLineParserErrorFormatter
	{
		string Format(ICommandLineParserError parserError);
		string Format(IEnumerable<ICommandLineParserError> parserErrors);
	}
}
using Fclp.Internals;

namespace Fclp
{
	public interface ICommandLineParserError
	{
		ICommandLineOption Option { get; }
	}
}
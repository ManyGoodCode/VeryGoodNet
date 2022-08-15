using System;

namespace Fclp.Internals
{
	public interface ICommandLineOptionFactory
	{
		ICommandLineOptionResult<T> CreateOption<T>(string shortName, string longName);
		IHelpCommandLineOptionResult CreateHelpOption(string[] helpArgs);
	}
}

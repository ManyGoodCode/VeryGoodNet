using System;

namespace Fclp
{
	public interface IHelpCommandLineOptionFluent
	{
		IHelpCommandLineOptionFluent Callback(Action<string> callback);
		IHelpCommandLineOptionFluent Callback(Action callback);
		IHelpCommandLineOptionFluent WithCustomFormatter(ICommandLineOptionFormatter formatter);
		IHelpCommandLineOptionFluent WithHeader(string header);
		IHelpCommandLineOptionFluent UseForEmptyArgs();
	}
}
using System;
using System.Collections.Generic;

namespace Fclp
{
	public interface ICommandLineOptionFluent<T>
	{
		ICommandLineOptionFluent<T> WithDescription(string description);
		ICommandLineOptionFluent<T> Required();
		ICommandLineOptionFluent<T> Callback(Action<T> callback);
        ICommandLineOptionFluent<T> SetDefault(T value);
		ICommandLineOptionFluent<T> CaptureAdditionalArguments(Action<IEnumerable<string>> callback);
        ICommandLineOptionFluent<T> AssignToCommand(ICommandLineCommand command);
	    ICommandLineOptionFluent<T> UseForOrphanArguments();
	}
}

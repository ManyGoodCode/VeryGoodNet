using System;
using System.Collections.Generic;
using Fclp.Internals;

namespace Fclp
{
	public interface ICommandLineOptionFormatter
	{
		string Format(IEnumerable<ICommandLineOption> options);
	}
}

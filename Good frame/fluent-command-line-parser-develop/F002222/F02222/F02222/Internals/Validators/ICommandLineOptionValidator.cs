using System;

namespace Fclp.Internals.Validators
{
	public interface ICommandLineOptionValidator
	{
	    void Validate(ICommandLineOption commandLineOption, StringComparison stringComparison);
	}
}
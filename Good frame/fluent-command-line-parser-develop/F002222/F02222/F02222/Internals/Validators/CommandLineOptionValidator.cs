using System;
using System.Collections.Generic;

namespace Fclp.Internals.Validators
{
	public class CommandLineOptionValidator : ICommandLineOptionValidator
	{
		private readonly IList<ICommandLineOptionValidator> rules;
        public CommandLineOptionValidator(ICommandLineOptionContainer container, SpecialCharacters specialCharacters)
		{
			rules = new List<ICommandLineOptionValidator>
			{
				new OptionNameValidator(specialCharacters),
				new NoDuplicateOptionValidator(container)
			};
		}

	    public void Validate(ICommandLineOption commandLineOption, StringComparison stringComparison)
		{
			foreach (ICommandLineOptionValidator rule in rules)
			{
				rule.Validate(commandLineOption, stringComparison);
			}
		}
	}
}
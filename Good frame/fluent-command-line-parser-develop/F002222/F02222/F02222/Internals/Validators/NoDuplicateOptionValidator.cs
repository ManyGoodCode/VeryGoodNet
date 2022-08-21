using System;
using System.Collections.Generic;
using System.Linq;

namespace Fclp.Internals.Validators
{
    public interface ICommandLineOptionContainer
    {
        IEnumerable<ICommandLineOption> Options { get; }
    }

	public class NoDuplicateOptionValidator : ICommandLineOptionValidator
	{
        private readonly ICommandLineOptionContainer container;
        public NoDuplicateOptionValidator(ICommandLineOptionContainer container)
		{
            if (container == null) 
                throw new ArgumentNullException("container");
            this.container = container;
		}

	    public void Validate(ICommandLineOption commandLineOption, StringComparison stringComparison)
		{
            foreach (ICommandLineOption option in container.Options)
			{
			    if (option.HasCommand)
			    {
			        if (CommandsAreEqual(option.Command, commandLineOption.Command, stringComparison))
			        {
                        if (string.IsNullOrEmpty(commandLineOption.ShortName) == false)
                        {
                            ValuesAreEqual(commandLineOption.ShortName, option.ShortName, stringComparison);
                        }

                        if (string.IsNullOrEmpty(commandLineOption.LongName) == false)
                        {
                            ValuesAreEqual(commandLineOption.LongName, option.LongName, stringComparison);
                        }
                    }
			    }
			    else
			    {
                    if (string.IsNullOrEmpty(commandLineOption.ShortName) == false)
                    {
                        ValuesAreEqual(commandLineOption.ShortName, option.ShortName, stringComparison);
                    }

                    if (string.IsNullOrEmpty(commandLineOption.LongName) == false)
                    {
                        ValuesAreEqual(commandLineOption.LongName, option.LongName, stringComparison);
                    }
                }
			
			}
		}

        private void ValuesAreEqual(string value, string otherValue, StringComparison stringComparison)
		{
            if (string.Equals(value, otherValue, stringComparison))
			{
				throw new OptionAlreadyExistsException(value);
			}
		}

        private bool CommandsAreEqual(
            ICommandLineCommand command, 
            ICommandLineCommand otherCommand, 
            StringComparison stringComparison)
        {
            if (command == null && otherCommand == null)
                return true;
            if (command == null) 
                return false;
            if (otherCommand == null) 
                return false;
            return string.Equals(command.Name, otherCommand.Name, stringComparison);
	    }
	}
}
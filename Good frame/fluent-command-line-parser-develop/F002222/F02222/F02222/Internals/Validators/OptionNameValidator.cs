using System;
using System.Globalization;
using System.Linq;

namespace Fclp.Internals.Validators
{
	public class OptionNameValidator : ICommandLineOptionValidator
	{
	    private readonly char[] reservedChars;
        public OptionNameValidator(SpecialCharacters specialCharacters)
	    {
	       reservedChars = specialCharacters.ValueAssignments.Union(new[] { specialCharacters.Whitespace }).ToArray();
        }


	    public void Validate(ICommandLineOption commandLineOption, StringComparison stringComparison)
		{
			if (commandLineOption == null) 
				throw new ArgumentNullException("commandLineOption");
            ValidateShortName(commandLineOption.ShortName);
			ValidateLongName(commandLineOption.LongName);
			ValidateShortAndLongName(commandLineOption.ShortName, commandLineOption.LongName);
		}

		private void ValidateShortAndLongName(string shortName, string longName)
		{
			if (string.IsNullOrEmpty(shortName) && string.IsNullOrEmpty(longName))
			{
				ThrowInvalid(string.Empty, "A short or long name must be provided.");
			}
		}

		private void ValidateLongName(string longName)
		{
			if (string.IsNullOrEmpty(longName)) 
				return;
			VerifyDoesNotContainsReservedChar(longName);
			if (longName.Length == 1)
			{
				ThrowInvalid(longName, "Long names must be longer than a single character. Single characters are reserved for short options only.");
			}
		}

		private void ValidateShortName(string shortName)
		{
			if (string.IsNullOrEmpty(shortName))
				return;
			if (shortName.Length > 1)
			{
				ThrowInvalid(shortName, "Short names must be a single character only.");
			}

			VerifyDoesNotContainsReservedChar(shortName);
			if (char.IsControl(shortName, 0))
			{
				ThrowInvalid(shortName, "The character '" + shortName + "' is not valid for a short name.");
			}
		}

		private void VerifyDoesNotContainsReservedChar(string value)
		{
			if (string.IsNullOrEmpty(value)) 
				return;
			foreach (char reservedChar in reservedChars)
			{
				if (value.Contains(reservedChar))
				{
					ThrowInvalid(value, "The character '" + reservedChar + "' is not valid within a short or long name.");
				}
			}
		}

		private static void ThrowInvalid(string value, string message)
		{
			throw new InvalidOptionNameException(
				string.Format(CultureInfo.InvariantCulture, "Invalid option name '{0}'. {1}", value, message));
		}
	}
}
using System.Collections.Generic;
using System.Text;
using Fclp.Internals.Errors;
using Fclp.Internals.Extensions;

namespace Fclp
{
	public class CommandLineParserErrorFormatter : ICommandLineParserErrorFormatter
	{
		public string Format(IEnumerable<ICommandLineParserError> parserErrors)
		{
			if (parserErrors.IsNullOrEmpty()) 
				return null;
			StringBuilder builder = new StringBuilder();
			foreach (ICommandLineParserError error in parserErrors)
			{
				builder.AppendLine(Format(error));
			}

			return builder.ToString();
		}

		public string Format(ICommandLineParserError parserError)
		{
			OptionSyntaxParseError optionSyntaxParseError = parserError as OptionSyntaxParseError;
			if (optionSyntaxParseError != null) 
				return FormatOptionSyntaxParseError(optionSyntaxParseError);
			ExpectedOptionNotFoundParseError expectedOptionNotFoundError = parserError as ExpectedOptionNotFoundParseError;
			if (expectedOptionNotFoundError != null)
				return FormatExpectedOptionNotFoundError(expectedOptionNotFoundError);
			return "unknown parse error.";
		}

		private static string FormatOptionSyntaxParseError(OptionSyntaxParseError error)
		{
			return string.Format("Option '{0}' parse error: could not parse '{1}' to '{2}'.",
			                     error.ParsedOption.RawKey,
								 error.ParsedOption.Value.RemoveAnyWrappingDoubleQuotes(),
			                     error.Option.SetupType);
		}

		private static string FormatExpectedOptionNotFoundError(ExpectedOptionNotFoundParseError error)
		{
			string optionText = GetOptionText(error);
			return string.Format("Option '{0}' parse error. option is required but was not specified.", optionText);
		}

		private static string GetOptionText(ICommandLineParserError error)
		{
			string optionText = error.Option.LongName.IsNullOrWhiteSpace()
				                 ? error.Option.ShortName
				                 : error.Option.ShortName + ":" + error.Option.LongName;
			return optionText;
		}
	}
}
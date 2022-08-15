using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Parsing;

namespace Fclp.Internals
{
	public class HelpCommandLineOption : IHelpCommandLineOptionResult
	{
		ICommandLineOptionFormatter _optionFormatter;
		public HelpCommandLineOption(IEnumerable<string> helpArgs)
		{
			HelpArgs = helpArgs ?? new List<string>();
		}

		public IEnumerable<string> HelpArgs { get; private set; }
		internal Action<string> ReturnCallback { get; set; }
		private Action ReturnCallbackWithoutParser { get; set; }
		private bool ShouldUseForEmptyArgs { get; set; }
		public string Header { get; set; }

		public ICommandLineOptionFormatter OptionFormatter
		{
			get { return _optionFormatter ?? (_optionFormatter = new CommandLineOptionFormatter { Header = this.Header }); }
			set { _optionFormatter = value; }
		}

		public IHelpCommandLineOptionFluent Callback(Action<string> callback)
		{
			ReturnCallback = callback;
			return this;
		}

		public IHelpCommandLineOptionFluent Callback(Action callback)
		{
			ReturnCallbackWithoutParser = callback;
			return this;
		}


		public IHelpCommandLineOptionFluent WithCustomFormatter(ICommandLineOptionFormatter formatter)
		{
			this.OptionFormatter = formatter;
			return this;
		}


		public IHelpCommandLineOptionFluent WithHeader(string header)
		{
			this.Header = header;
			return this;
		}


		public IHelpCommandLineOptionFluent UseForEmptyArgs()
		{
			this.ShouldUseForEmptyArgs = true;
			return this;
		}

		public bool ShouldShowHelp(IEnumerable<ParsedOption> parsedOptions, StringComparison comparisonType)
		{
			List<ParsedOption> parsed = parsedOptions != null ? parsedOptions.ToList() : new List<ParsedOption>();
			if (parsed.Any() == false && ShouldUseForEmptyArgs)
			{
				return true;
			}

			return this.HelpArgs.Any(helpArg => parsed.Any(cmdArg => helpArg.Equals(cmdArg.Key, comparisonType)));
		}

		public void ShowHelp(IEnumerable<ICommandLineOption> options)
		{
			if (ReturnCallback != null)
			{
				var formattedOutput = this.OptionFormatter.Format(options);
				this.ReturnCallback(formattedOutput);    
			}

			if (ReturnCallbackWithoutParser != null)
			{
				this.ReturnCallbackWithoutParser();
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Fclp.Internals;
using Fclp.Internals.Errors;
using Fclp.Internals.Extensions;
using Fclp.Internals.Parsing;
using Fclp.Internals.Validators;

namespace Fclp
{
	public class FluentCommandLineParser : IFluentCommandLineParser
	{
		public FluentCommandLineParser()
		{
			IsCaseSensitive = true;
		}

		public const StringComparison CaseSensitiveComparison = StringComparison.CurrentCulture;
		public const StringComparison IgnoreCaseComparison = StringComparison.CurrentCultureIgnoreCase;

		List<ICommandLineOption> _options;
        List<ICommandLineCommand> _commands;
		ICommandLineOptionFactory _optionFactory;
		ICommandLineParserEngine _parserEngine;
		ICommandLineOptionFormatter _optionFormatter;
		IHelpCommandLineOption _helpOption;
		ICommandLineParserErrorFormatter _errorFormatter;
		ICommandLineOptionValidator _optionValidator;
	    SpecialCharacters _specialCharacters;

		public bool IsCaseSensitive
		{
			get { return StringComparison == CaseSensitiveComparison; }
			set { StringComparison = value ? CaseSensitiveComparison : IgnoreCaseComparison; }
		}

        public IFluentCommandLineParser MakeCaseInsensitive()
	    {
	        IsCaseSensitive = false;
	        return this;
	    }

        public IFluentCommandLineParser DisableShortOptions()
	    {
            SpecialCharacters.ShortOptionPrefix.Clear();
	        return this;
	    }

        public IFluentCommandLineParser UseOwnOptionPrefix(params string[] prefix)
	    {
	        SpecialCharacters.OptionPrefix.Clear();
	        SpecialCharacters.OptionPrefix.AddRange(prefix);
	        return this;
	    }

        public IFluentCommandLineParser SkipFirstArg()
	    {
	        SkipTheFirstArg = true;
	        return this;
	    }

		internal StringComparison StringComparison { get; private set; }
        internal bool SkipTheFirstArg { get; set; }
		public List<ICommandLineOption> Options
		{
			get { return _options ?? (_options = new List<ICommandLineOption>()); }
		}

	    public List<ICommandLineCommand> Commands
	    {
            get { return _commands ?? (_commands = new List<ICommandLineCommand>()); }
	    }

        public ParseSequence ParseSequence { get; set; }
		public ICommandLineOptionFormatter OptionFormatter
		{
			get { return _optionFormatter ?? (_optionFormatter = new CommandLineOptionFormatter()); }
			set { _optionFormatter = value; }
		}


		public ICommandLineParserErrorFormatter ErrorFormatter
		{
			get { return _errorFormatter ?? (_errorFormatter = new CommandLineParserErrorFormatter()); }
			set { _errorFormatter = value; }
		}

		public ICommandLineOptionFactory OptionFactory
		{
			get { return _optionFactory ?? (_optionFactory = new CommandLineOptionFactory()); }
			set { _optionFactory = value; }
		}

		public ICommandLineOptionValidator OptionValidator
		{
			get { return _optionValidator ?? (_optionValidator = new CommandLineOptionValidator(this, SpecialCharacters)); }
			set { _optionValidator = value; }
		}

		public ICommandLineParserEngine ParserEngine
		{
			get { return _parserEngine ?? (_parserEngine = new CommandLineParserEngineMark2(SpecialCharacters)); }
			set { _parserEngine = value; }
		}

        public IHelpCommandLineOption HelpOption
		{
			get { return _helpOption ?? (_helpOption = new EmptyHelpCommandLineOption()); }
			set { _helpOption = value; }
		}

	    public bool HasCommands
	    {
            get { return Commands.Any(); }
	    }

	    public SpecialCharacters SpecialCharacters
	    {
	        get { return _specialCharacters ?? (_specialCharacters = new SpecialCharacters()); }
	    }

        public ICommandLineOptionFluent<T> Setup<T>(char shortOption, string longOption)
		{
			return SetupInternal<T>(shortOption.ToString(CultureInfo.InvariantCulture), longOption);
		}

		[Obsolete("Use new overload Setup<T>(char, string) to specify both a short and long option name instead.")]
		public ICommandLineOptionFluent<T> Setup<T>(string shortOption, string longOption)
		{
			return SetupInternal<T>(shortOption, longOption);
		}

		private ICommandLineOptionFluent<T> SetupInternal<T>(string shortOption, string longOption)
		{
			ICommandLineOptionResult<T> argOption = this.OptionFactory.CreateOption<T>(shortOption, longOption);
			if (argOption == null)
				throw new InvalidOperationException("OptionFactory is producing unexpected results.");

			OptionValidator.Validate(argOption, IsCaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
			this.Options.Add(argOption);
			return argOption;
		}

		public ICommandLineOptionFluent<T> Setup<T>(char shortOption)
		{
			return SetupInternal<T>(shortOption.ToString(CultureInfo.InvariantCulture), null);
		}

		public ICommandLineOptionFluent<T> Setup<T>(string longOption)
		{
			return SetupInternal<T>(null, longOption);
		}

        public ICommandLineCommandFluent<TBuildType> SetupCommand<TBuildType>(string name) where TBuildType : new()
        {
			CommandLineCommand<TBuildType> command = new CommandLineCommand<TBuildType>(this) { Name = name };
            Commands.Add(command);
            return command;
        }

		public ICommandLineParserResult Parse(string[] args)
		{
		    if (SkipTheFirstArg) 
				args = args.Skip(1).ToArray();
			ParserEngineResult parserEngineResult = this.ParserEngine.Parse(args, HasCommands);
			List<ParsedOption> parsedOptions = parserEngineResult.ParsedOptions.ToList();
			CommandLineParserResult result = new CommandLineParserResult { EmptyArgs = parsedOptions.IsNullOrEmpty(), RawResult = parserEngineResult };
			if (this.HelpOption.ShouldShowHelp(parsedOptions, StringComparison))
			{
				result.HelpCalled = true;
				this.HelpOption.ShowHelp(this.Options);
				return result;
			}

		    if (parserEngineResult.HasCommand)
		    {
		        ICommandLineCommand match = Commands.SingleOrDefault(cmd => cmd.Name.Equals(parserEngineResult.Command, this.StringComparison));
		        if (match != null)
		        {
                    ICommandLineParserResult result2 = ParseOptions(match.Options, parsedOptions, result);
		            if (result2.HasErrors == false)
		            {
                        match.ExecuteOnSuccess();    
		            }
		            return result2;
		        }
		    }

            return ParseOptions(this.Options, parsedOptions, result);
		}

	    private ICommandLineParserResult ParseOptions(IEnumerable<ICommandLineOption> options, List<ParsedOption> parsedOptions, CommandLineParserResult result)
	    {
			HashSet<ParsedOption> matchedOptions = new HashSet<ParsedOption>();
	        int optionIndex = 0;
	        foreach (ICommandLineOption setupOption in options)
	        {
	            ICommandLineOption option = setupOption;
	            int matchIndex = parsedOptions.FindIndex(pair =>
	                    !matchedOptions.Contains(pair) &&
	                    (pair.Key.Equals(option.ShortName, this.StringComparison) 
	                     || pair.Key.Equals(option.LongName, this.StringComparison))
	            );

	            if (matchIndex > -1) 
	            {
					ParsedOption match = parsedOptions[matchIndex];
	                match.Order = matchIndex;
	                match.SetupCommand = option;
	                match.SetupOrder = optionIndex++;
	                matchedOptions.Add(match);
	            }
	            else if (setupOption.UseForOrphanArgs && result.RawResult.AdditionalValues.Any())
	            {
	                try
	                {
						OptionArgumentParser parser = new OptionArgumentParser(SpecialCharacters);
						ParsedOption blankOption = new ParsedOption();
	                    parser.ParseArguments(result.RawResult.AdditionalValues, blankOption);
	                    setupOption.Bind(blankOption);
	                }
	                catch (OptionSyntaxException)
	                {
	                    result.Errors.Add(new OptionSyntaxParseError(option, null));
	                    if (option.HasDefault)
	                        option.BindDefault();
	                }
	            }
                else
	            {
	                if (option.IsRequired) 
	                    result.Errors.Add(new ExpectedOptionNotFoundParseError(option));
	                else if (option.HasDefault)
	                    option.BindDefault(); 
	                result.UnMatchedOptions.Add(option);
	            }

	        }

	        foreach (var match in ParseSequence == ParseSequence.SameAsSetup ? matchedOptions.OrderBy(o => o.SetupOrder) : matchedOptions.OrderBy(o => o.Order))
	        {
	            try
	            {
	                match.SetupCommand.Bind(match);
	            }
	            catch (OptionSyntaxException)
	            {
	                result.Errors.Add(new OptionSyntaxParseError(match.SetupCommand, match));
	                if (match.SetupCommand.HasDefault)
	                    match.SetupCommand.BindDefault();
	            }
	        }

	        parsedOptions.Where(item => !matchedOptions.Contains(item))
				.ForEach(item => result.AdditionalOptionsFound.Add(new KeyValuePair<string, string>(item.Key, item.Value)));
	        result.ErrorText = ErrorFormatter.Format(result.Errors);
	        return result;
        }

        public IHelpCommandLineOptionFluent SetupHelp(params string[] helpArgs)
		{
			IHelpCommandLineOptionResult helpOption = this.OptionFactory.CreateHelpOption(helpArgs);
			this.HelpOption = helpOption;
			return helpOption;
		}

		IEnumerable<ICommandLineOption> ICommandLineOptionContainer.Options
		{
			get { return Options; }
		}
	}
}

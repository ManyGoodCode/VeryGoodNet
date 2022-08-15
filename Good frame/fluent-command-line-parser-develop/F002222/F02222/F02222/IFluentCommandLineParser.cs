using System;
using System.Collections.Generic;
using Fclp.Internals;
using Fclp.Internals.Validators;

namespace Fclp
{
	public interface IFluentCommandLineParser : ICommandLineOptionSetupFactory, ICommandLineOptionContainer
	{
		[Obsolete("Use new overload Setup<T>(char, string) to specify both a short and long option name instead.")]
		ICommandLineOptionFluent<T> Setup<T>(string shortOption, string longOption);
	    ICommandLineCommandFluent<TBuildType> SetupCommand<TBuildType>(string name) where TBuildType : new();
		IHelpCommandLineOptionFluent SetupHelp(params string[] helpArgs);
		ICommandLineParserResult Parse(string[] args);
        IHelpCommandLineOption HelpOption { get; set; }
		bool IsCaseSensitive { get; set; }
        SpecialCharacters SpecialCharacters { get; }
	    IFluentCommandLineParser MakeCaseInsensitive();
	    IFluentCommandLineParser DisableShortOptions();
	    IFluentCommandLineParser UseOwnOptionPrefix(params string[] prefix);
	    IFluentCommandLineParser SkipFirstArg();
    }
}

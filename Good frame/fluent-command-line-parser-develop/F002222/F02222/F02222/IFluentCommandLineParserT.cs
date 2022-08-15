using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Fclp.Internals;

namespace Fclp
{
	public interface IFluentCommandLineParser<TBuildType> where TBuildType : class
	{
		TBuildType Object { get; }
		ICommandLineOptionBuilderFluent<TProperty> Setup<TProperty>(Expression<Func<TBuildType, TProperty>> propertyPicker);
		ICommandLineParserResult Parse(string[] args);
		IHelpCommandLineOptionFluent SetupHelp(params string[] helpArgs);
		bool IsCaseSensitive { get; set; }
        IEnumerable<ICommandLineOption> Options { get; }
        IFluentCommandLineParser<TBuildType> MakeCaseInsensitive();
        IFluentCommandLineParser<TBuildType> DisableShortOptions();
        IFluentCommandLineParser<TBuildType> UseOwnOptionPrefix(params string[] prefix);
	    IFluentCommandLineParser<TBuildType> SkipFirstArg();
	}
}
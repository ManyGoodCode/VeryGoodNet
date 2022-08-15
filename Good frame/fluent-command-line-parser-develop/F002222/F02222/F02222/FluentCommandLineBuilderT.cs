using System;

namespace Fclp
{
	[Obsolete("FluentCommandLineBuilder<TBuildType> has been renamed to FluentCommandLineParser<TBuildType>", false)]
	public class FluentCommandLineBuilder<TBuildType> : FluentCommandLineParser<TBuildType>, IFluentCommandLineBuilder<TBuildType> where TBuildType : class, new()
	{
		 
	}
}
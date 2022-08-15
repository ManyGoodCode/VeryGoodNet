using System;

namespace Fclp
{
    [Obsolete("IFluentCommandLineBuilder<TBuildType> has been renamed to IFluentCommandLineParser<TBuildType>", false)]
	public interface IFluentCommandLineBuilder<TBuildType> : IFluentCommandLineParser<TBuildType> where TBuildType : class
	{
		 
	}
}
using System;
using Fclp.Internals.Parsing;

namespace Fclp.Internals
{
	public interface ICommandLineOption
	{
		bool IsRequired { get; }
		string Description { get; }
		void Bind(ParsedOption value);
		void BindDefault();
		string ShortName { get; }
		string LongName { get; }
		bool HasLongName { get; }
		bool HasShortName { get; }
		bool HasCallback { get; }
		bool HasAdditionalArgumentsCallback { get; }
		bool HasDefault { get; }
		Type SetupType { get; }
        ICommandLineCommand Command { get; set; }
        bool UseForOrphanArgs { get; }
	    bool HasCommand { get; }
	    object GetDefaultValue();
	}
}

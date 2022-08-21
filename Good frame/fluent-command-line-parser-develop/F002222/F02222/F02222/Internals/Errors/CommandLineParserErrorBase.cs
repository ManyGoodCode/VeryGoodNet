using System;

namespace Fclp.Internals.Errors
{
	/// <summary>
	/// ��װ��Ա����  ICommandLineOption 
	/// </summary>
	public abstract class CommandLineParserErrorBase : ICommandLineParserError
	{
		protected CommandLineParserErrorBase(ICommandLineOption cmdOption)
		{
			if (cmdOption == null) 
				throw new ArgumentNullException("cmdOption");
			this.Option = cmdOption;
		}

		public virtual ICommandLineOption Option { get; private set; }
	}
}
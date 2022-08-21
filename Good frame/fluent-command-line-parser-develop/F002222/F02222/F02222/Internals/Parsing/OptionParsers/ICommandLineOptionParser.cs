namespace Fclp.Internals.Parsing.OptionParsers
{
	/// <summary>
	/// 解析器实现的接口
	/// 
	/// </summary>
	public interface ICommandLineOptionParser<T>
	{
		T Parse(ParsedOption parsedOption);

		bool CanParse(ParsedOption parsedOption);
	}
}

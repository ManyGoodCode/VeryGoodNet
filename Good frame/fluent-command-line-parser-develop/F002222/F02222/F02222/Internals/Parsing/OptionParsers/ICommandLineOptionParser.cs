namespace Fclp.Internals.Parsing.OptionParsers
{
	/// <summary>
	/// ������ʵ�ֵĽӿ�
	/// 
	/// </summary>
	public interface ICommandLineOptionParser<T>
	{
		T Parse(ParsedOption parsedOption);

		bool CanParse(ParsedOption parsedOption);
	}
}

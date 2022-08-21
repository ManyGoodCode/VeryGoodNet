using System.Collections.Generic;

namespace Fclp.Internals
{
	public class SpecialCharacters
	{
		/// <summary>
		/// 赋值字符  '=', ':'
		/// </summary>
		public char[] ValueAssignments { get; private set; } = new[] { '=', ':' };

		public char Whitespace { get; set; } = ' ';

		/// <summary>
		/// 前缀字符 "/", "--", "-"
		/// </summary>
		public List<string> OptionPrefix { get; private set; } = new List<string> { "/", "--", "-" };

		/// <summary>
		/// 后缀字符 "+", "-"
		/// </summary>
		public List<string> OptionSuffix { get; private set; } = new List<string> { "+", "-" };

		/// <summary>
		/// 短前缀 "-"
		/// </summary>
		public List<string> ShortOptionPrefix { get; private set; } = new List<string> { "-" };

		/// <summary>
		/// 结束字符 "--"
		/// </summary>
		public string EndOfOptionsKey { get; set; } = "--";
	}
}

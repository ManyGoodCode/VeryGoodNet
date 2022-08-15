using System.Collections.Generic;

namespace Fclp.Internals
{
	public class SpecialCharacters
	{
		public char[] ValueAssignments { get; private set; } = new[] { '=', ':' };
		public char Whitespace { get; set; } = ' ';
		public List<string> OptionPrefix { get; private set; } = new List<string> { "/", "--", "-" };
		public List<string> OptionSuffix { get; private set; } = new List<string> { "+", "-" };
		public List<string> ShortOptionPrefix { get; private set; } = new List<string> { "-" };
		public string EndOfOptionsKey { get; set; } = "--";
	}
}

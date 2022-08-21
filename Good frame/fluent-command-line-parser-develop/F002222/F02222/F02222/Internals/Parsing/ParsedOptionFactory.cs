using System.Linq;

namespace Fclp.Internals.Parsing
{
	/// <summary>
	/// 根据前缀和后缀 生成 ParsedOption 
	/// </summary>
	public class ParsedOptionFactory
	{
	    private readonly SpecialCharacters specialCharacters;

	    public ParsedOptionFactory(SpecialCharacters specialCharacters)
	    {
	        this.specialCharacters = specialCharacters;
	    }

		public ParsedOption Create(string rawKey)
		{
			string prefix = ExtractPrefix(rawKey);
			return new ParsedOption
			{
				RawKey = rawKey,
				Prefix = prefix,
				Key = rawKey.Remove(0, prefix.Length),
				Suffix = ExtractSuffix(rawKey)
			};			
		}

		/// <summary>
		/// 提取前缀 ： "/", "--", "-"
		/// </summary>
		private string ExtractPrefix(string arg)
		{
			// 注意此处委托的使用
			return arg != null ? specialCharacters.OptionPrefix.FirstOrDefault(arg.StartsWith) : null;
		}

		/// <summary>
		/// 提取后缀 : "/", "--", "-"
		/// </summary>
		private string ExtractSuffix(string arg)
		{
			return arg != null ? specialCharacters.OptionSuffix.FirstOrDefault(arg.EndsWith) : null;
		}
	}
}
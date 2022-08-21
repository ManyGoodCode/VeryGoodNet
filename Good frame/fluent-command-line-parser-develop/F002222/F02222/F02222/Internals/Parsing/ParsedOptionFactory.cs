using System.Linq;

namespace Fclp.Internals.Parsing
{
	/// <summary>
	/// ����ǰ׺�ͺ�׺ ���� ParsedOption 
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
		/// ��ȡǰ׺ �� "/", "--", "-"
		/// </summary>
		private string ExtractPrefix(string arg)
		{
			// ע��˴�ί�е�ʹ��
			return arg != null ? specialCharacters.OptionPrefix.FirstOrDefault(arg.StartsWith) : null;
		}

		/// <summary>
		/// ��ȡ��׺ : "/", "--", "-"
		/// </summary>
		private string ExtractSuffix(string arg)
		{
			return arg != null ? specialCharacters.OptionSuffix.FirstOrDefault(arg.EndsWith) : null;
		}
	}
}
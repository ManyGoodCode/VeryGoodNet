using System.Linq;

namespace Fclp.Internals.Parsing
{
	public class ParsedOptionFactory
	{
	    private readonly SpecialCharacters _specialCharacters;

	    public ParsedOptionFactory(SpecialCharacters specialCharacters)
	    {
	        _specialCharacters = specialCharacters;
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


		private string ExtractPrefix(string arg)
		{
			return arg != null ? _specialCharacters.OptionPrefix.FirstOrDefault(arg.StartsWith) : null;
		}

		private string ExtractSuffix(string arg)
		{
			return arg != null ? _specialCharacters.OptionSuffix.FirstOrDefault(arg.EndsWith) : null;
		}
	}
}
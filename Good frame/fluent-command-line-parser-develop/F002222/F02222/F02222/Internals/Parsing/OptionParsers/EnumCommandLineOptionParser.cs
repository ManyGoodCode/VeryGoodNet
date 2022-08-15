using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing.OptionParsers
{
	public class EnumCommandLineOptionParser<TEnum> : ICommandLineOptionParser<TEnum>
	{
		private readonly IList<TEnum> _all;
		private readonly Dictionary<string, TEnum> _insensitiveNames;
		private readonly Dictionary<int, TEnum> _values;

		public EnumCommandLineOptionParser()
		{
			Type type = typeof(TEnum);
			if (!type.IsEnum) 
				throw new ArgumentException(string.Format("T must be an System.Enum but is '{0}'", type));
			_all = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
			_insensitiveNames = _all.ToDictionary(k => Enum.GetName(typeof(TEnum), k).ToLowerInvariant());
			_values = _all.ToDictionary(k => Convert.ToInt32(k));
		}

		public TEnum Parse(ParsedOption parsedOption)
		{
			return (TEnum)Enum.Parse(typeof(TEnum), parsedOption.Value.ToLowerInvariant(), true);
		}

		public bool CanParse(ParsedOption parsedOption)
		{
			if (parsedOption.HasValue == false) 
				return false;
			if (parsedOption.Value.IsNullOrWhiteSpace()) 
				return false;
			return IsDefined(parsedOption.Value);
		}

		private bool IsDefined(string value)
		{
			int asInt;
			return int.TryParse(value, out asInt) 
				? IsDefined(asInt) 
				: _insensitiveNames.Keys.Contains(value.ToLowerInvariant());
		}

		private bool IsDefined(int value)
		{
			return _values.Keys.Contains(value);
		}
	}
}
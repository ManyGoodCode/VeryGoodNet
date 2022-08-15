using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing
{
	public class OptionArgumentParser
	{
	    private readonly SpecialCharacters _specialCharacters;
        public OptionArgumentParser(SpecialCharacters specialCharacters)
	    {
	        _specialCharacters = specialCharacters;
	    }

		public void ParseArguments(IEnumerable<string> args, ParsedOption option)
		{
			if (option.Key != null && _specialCharacters.ValueAssignments.Any(option.Key.Contains))
			{
				TryGetArgumentFromKey(option);
			}

			List<string> allArguments = new List<string>();
			List<string> additionalArguments = new List<string>();

			List<string> otherArguments = CollectArgumentsUntilNextKey(args).ToList();

			if (option.HasValue) 
				allArguments.Add(option.Value);
			if (otherArguments.Any())
			{
				allArguments.AddRange(otherArguments);
				if (otherArguments.Count() > 1)
				{
					additionalArguments.AddRange(otherArguments);
					additionalArguments.RemoveAt(0);
				}
			}

			option.Value = allArguments.FirstOrDefault();
			option.Values = allArguments.ToArray();
			option.AdditionalValues = additionalArguments.ToArray();
		}

		private void TryGetArgumentFromKey(ParsedOption option)
		{
			string[] split = option.Key.Split(_specialCharacters.ValueAssignments, 2, StringSplitOptions.RemoveEmptyEntries);
			option.Key = split[0];
			option.Value = split.Length > 1 
				               ? split[1].WrapInDoubleQuotesIfContainsWhitespace()
				               : null;
		}

	    private IEnumerable<string> CollectArgumentsUntilNextKey(IEnumerable<string> args)
		{
			return from argument in args
			       where !IsEndOfOptionsKey(argument)
			       select argument.WrapInDoubleQuotesIfContainsWhitespace();
		}

        private bool IsEndOfOptionsKey(string arg)
		{
			return string.Equals(arg, _specialCharacters.EndOfOptionsKey, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
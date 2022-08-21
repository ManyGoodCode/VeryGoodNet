using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing
{
	public class OptionArgumentParser
	{
	    private readonly SpecialCharacters specialCharacters;
        public OptionArgumentParser(SpecialCharacters specialCharacters)
	    {
	        this.specialCharacters = specialCharacters;
	    }

		/// <summary>
		/// 解析语句 Key-Value 到  ParsedOption
		/// </summary>
		public void ParseArguments(IEnumerable<string> args, ParsedOption option)
		{
			if (option.Key != null && specialCharacters.ValueAssignments.Any(option.Key.Contains))
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

		/// <summary>
		/// 为  ParsedOption 赋值语句 加载  Key 和 Value
		/// </summary>
		private void TryGetArgumentFromKey(ParsedOption option)
		{
			string[] split = option.Key.Split(specialCharacters.ValueAssignments, 2, StringSplitOptions.RemoveEmptyEntries);
			option.Key = split[0];
			option.Value = split.Length > 1 
				               ? split[1].WrapInDoubleQuotesIfContainsWhitespace()
				               : null;
		}

		/// <summary>
		/// 包装值为 "" 
		/// </summary>
	    private IEnumerable<string> CollectArgumentsUntilNextKey(IEnumerable<string> args)
		{
			return from argument in args
			       where !IsEndOfOptionsKey(argument)
			       select argument.WrapInDoubleQuotesIfContainsWhitespace();
		}

		/// <summary>
		/// 判断是否为结束字符
		/// </summary>
        private bool IsEndOfOptionsKey(string arg)
		{
			return string.Equals(arg, specialCharacters.EndOfOptionsKey, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing
{
	public class CommandLineParserEngineMark2 : ICommandLineParserEngine
	{
	    private readonly SpecialCharacters specialCharacters;
	    private readonly List<string> additionalArgumentsFound = new List<string>();
		private readonly List<ParsedOption> parsedOptions = new List<ParsedOption>();
	    private readonly OptionArgumentParser optionArgumentParser;


        public CommandLineParserEngineMark2(SpecialCharacters specialCharacters)
	    {
	        this.specialCharacters = specialCharacters;
			this.optionArgumentParser = new OptionArgumentParser(specialCharacters);
        }

        public ParserEngineResult Parse(string[] args, bool parseCommands)
		{
			args = args ?? new string[0];
			CommandLineOptionGrouper grouper = new CommandLineOptionGrouper(specialCharacters);
            string[][] grouped = grouper.GroupArgumentsByOption(args, parseCommands);
            string command = parseCommands ? ExtractAnyCommand(grouped) : null;
            foreach (string[] optionGroup in grouped)
			{
				string rawKey = optionGroup.First();
				ParseGroupIntoOption(rawKey, optionGroup.Skip(1));
			}

            if (command != null)
            {
                additionalArgumentsFound.RemoveAt(0);
            }

			return new ParserEngineResult(parsedOptions, additionalArgumentsFound, command);
		}

	    private string ExtractAnyCommand(string[][] grouped)
	    {
	        if (grouped.Length > 0)
	        {
	            string[] cmdGroup = grouped.First();
	            string cmd = cmdGroup.FirstOrDefault();
	            if (IsAKey(cmd) == false)
	            {
	                return cmd;
	            }
	        }

	        return null;
	    }


	    private void ParseGroupIntoOption(string rawKey, IEnumerable<string> optionGroup)
		{
			if (IsAKey(rawKey))
			{
				ParsedOption parsedOption = new ParsedOptionFactory(specialCharacters).Create(rawKey);
				TrimSuffix(parsedOption);
				optionArgumentParser.ParseArguments(optionGroup, parsedOption);
				AddParsedOptionToList(parsedOption);
			}
			else
			{
				AddAdditionArgument(rawKey);
				optionGroup.ForEach(AddAdditionArgument);
			}
		}

		/// <summary>
		/// 拷贝或者直接添加 ParsedOption 到集合
		/// </summary>
		private void AddParsedOptionToList(ParsedOption parsedOption)
		{
			if (ShortOptionNeedsToBeSplit(parsedOption))
			{
				parsedOptions.AddRange(CloneAndSplit(parsedOption));
			}
			else
			{
				parsedOptions.Add(parsedOption);
			}
		}

		/// <summary>
		/// 如果参数不以结束字符，则添加到参数集合
		/// </summary>
		private void AddAdditionArgument(string argument)
		{
			if (IsEndOfOptionsKey(argument) == false)
			{
				additionalArgumentsFound.Add(argument);
			}
		}

		/// <summary>
		/// 判断 ParsedOption 为短前缀且Key长度大于1
		/// </summary>
		private bool ShortOptionNeedsToBeSplit(ParsedOption parsedOption)
		{
			return PrefixIsShortOption(parsedOption.Prefix) && parsedOption.Key.Length > 1;
		}

		/// <summary>
		/// 静态克隆  ParsedOption Key为之前Key的第一个字符
		/// </summary>
		private static IEnumerable<ParsedOption> CloneAndSplit(ParsedOption parsedOption)
		{
			return parsedOption.Key.Select(c => Clone(parsedOption, c)).ToList();
		}

		/// <summary>
		/// 静态克隆  ParsedOption Key为传入的字符
		/// </summary>
		private static ParsedOption Clone(ParsedOption toClone, char c)
		{
			ParsedOption clone = toClone.Clone();
			clone.Key = new string(new[] { c });
			return clone;
		}

		/// <summary>
		/// 判断是否包含短前缀
		/// </summary>
		private bool PrefixIsShortOption(string key)
		{
			return specialCharacters.ShortOptionPrefix.Contains(key);
		}

		private static void TrimSuffix(ParsedOption parsedOption)
		{
			if (parsedOption.HasSuffix)
			{
				parsedOption.Key = parsedOption.Key.TrimEnd(parsedOption.Suffix.ToCharArray());
			}
		}

		/// <summary>
		/// 判断是否开始于前缀符号或者等于前缀符号 "/", "--", "-"
		/// </summary>
		private bool IsAKey(string arg)
		{ 
			return arg != null 
				&& specialCharacters.OptionPrefix.Any(arg.StartsWith)
				&& specialCharacters.OptionPrefix.Any(arg.Equals) == false;
		}

		/// <summary>
		/// 判断是否以结束符  "--"
		/// </summary>
		private bool IsEndOfOptionsKey(string arg)
		{
			return string.Equals(arg, specialCharacters.EndOfOptionsKey, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
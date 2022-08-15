using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing
{
	public class CommandLineParserEngineMark2 : ICommandLineParserEngine
	{
	    private readonly SpecialCharacters _specialCharacters;
	    private readonly List<string> _additionalArgumentsFound = new List<string>();
		private readonly List<ParsedOption> _parsedOptions = new List<ParsedOption>();
	    private readonly OptionArgumentParser _optionArgumentParser;


        public CommandLineParserEngineMark2(SpecialCharacters specialCharacters)
	    {
	        _specialCharacters = specialCharacters;
	        _optionArgumentParser = new OptionArgumentParser(specialCharacters);
        }

        public ParserEngineResult Parse(string[] args, bool parseCommands)
		{
			args = args ?? new string[0];
			CommandLineOptionGrouper grouper = new CommandLineOptionGrouper(_specialCharacters);
            string[][] grouped = grouper.GroupArgumentsByOption(args, parseCommands);
            string command = parseCommands ? ExtractAnyCommand(grouped) : null;
            foreach (string[] optionGroup in grouped)
			{
				string rawKey = optionGroup.First();
				ParseGroupIntoOption(rawKey, optionGroup.Skip(1));
			}

            if (command != null)
            {
                _additionalArgumentsFound.RemoveAt(0);
            }

			return new ParserEngineResult(_parsedOptions, _additionalArgumentsFound, command);
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
				ParsedOption parsedOption = new ParsedOptionFactory(_specialCharacters).Create(rawKey);
				TrimSuffix(parsedOption);
				_optionArgumentParser.ParseArguments(optionGroup, parsedOption);
				AddParsedOptionToList(parsedOption);
			}
			else
			{
				AddAdditionArgument(rawKey);
				optionGroup.ForEach(AddAdditionArgument);
			}
		}

		private void AddParsedOptionToList(ParsedOption parsedOption)
		{
			if (ShortOptionNeedsToBeSplit(parsedOption))
			{
				_parsedOptions.AddRange(CloneAndSplit(parsedOption));
			}
			else
			{
				_parsedOptions.Add(parsedOption);
			}
		}

		private void AddAdditionArgument(string argument)
		{
			if (IsEndOfOptionsKey(argument) == false)
			{
				_additionalArgumentsFound.Add(argument);
			}
		}

		private bool ShortOptionNeedsToBeSplit(ParsedOption parsedOption)
		{
			return PrefixIsShortOption(parsedOption.Prefix) && parsedOption.Key.Length > 1;
		}

		private static IEnumerable<ParsedOption> CloneAndSplit(ParsedOption parsedOption)
		{
			return parsedOption.Key.Select(c => Clone(parsedOption, c)).ToList();
		}

		private static ParsedOption Clone(ParsedOption toClone, char c)
		{
			ParsedOption clone = toClone.Clone();
			clone.Key = new string(new[] { c });
			return clone;
		}

		private bool PrefixIsShortOption(string key)
		{
			return _specialCharacters.ShortOptionPrefix.Contains(key);
		}

		private static void TrimSuffix(ParsedOption parsedOption)
		{
			if (parsedOption.HasSuffix)
			{
				parsedOption.Key = parsedOption.Key.TrimEnd(parsedOption.Suffix.ToCharArray());
			}
		}

		private bool IsAKey(string arg)
		{ 
			return arg != null 
				&& _specialCharacters.OptionPrefix.Any(arg.StartsWith)
				&& _specialCharacters.OptionPrefix.Any(arg.Equals) == false;
		}

		private bool IsEndOfOptionsKey(string arg)
		{
			return string.Equals(arg, _specialCharacters.EndOfOptionsKey, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
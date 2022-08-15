using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing
{
    public class CommandLineOptionGrouper
    {
        private readonly SpecialCharacters _specialCharacters;
        private string[] _args;
        private int _currentOptionLookupIndex;
        private int[] _foundOptionLookup;
        private int _currentOptionIndex;
        private readonly List<string> _orphanArgs;
        private bool _parseCommands = false;

	    public CommandLineOptionGrouper(SpecialCharacters specialCharacters)
        {
            _specialCharacters = specialCharacters;
            _orphanArgs = new List<string>();
        }

        public string[][] GroupArgumentsByOption(string[] args, bool parseCommands)
        {
            if (args.IsNullOrEmpty()) 
                return new string[0][];
            _parseCommands = parseCommands;
            _args = args;
            _currentOptionIndex = -1;
            _currentOptionLookupIndex = -1;
            List<string[]> options = new List<string[]>();
            string first = _args.First();
            if (IsEndOfOptionsKey(first))
            {
                options.Add(CreateGroupForCurrent());
            }
            else
            {
                if (parseCommands && IsACmd(first))
                {
                    if (ContainsAtLeastOneOption(args))
                    {
                        options.Add(new[] {first});
                        FindOptionIndexes();
                        while (MoveToNextOption())
                        {
                            options.Add(CreateGroupForCurrent());
                        }
                    }
                    else
                    {
                        options.Add(CreateGroupForCurrent());
                    }
                }
                else
                {
                    FindOptionIndexes();
                    while (MoveToNextOption())
                    {
                        options.Add(CreateGroupForCurrent());
                    }
                }
            }

            if (_orphanArgs.Any())
            {
                if (options.Count > 0)
                {
                    options.Insert(1, _orphanArgs.ToArray());
                }
                else
                {
                    options.Add(_orphanArgs.ToArray());
                }
            }

            return options.ToArray();
        }

        private bool ContainsAtLeastOneOption(string[] args)
        {
            return args.Any(IsAKey);
        }

        private string[] CreateGroupForCurrent()
        {
            int optionEndIndex = LookupTheNextOptionIndex();
            optionEndIndex = optionEndIndex != -1
				? optionEndIndex - 1
				: _args.Length - 1;

			int length = optionEndIndex - (_currentOptionIndex - 1);
            return _args.Skip(_currentOptionIndex)
                        .Take(length)
                        .ToArray();
        }

        private void FindOptionIndexes()
        {
            List<int> indexes = new List<int>();
            for (int index = 0; index < _args.Length; index++)
            {
                string currentArg = _args[index];
                if (IsEndOfOptionsKey(currentArg)) 
                    break;
                if(_parseCommands && index == 0 && IsACmd(currentArg)) 
                    continue;
                if (IsAKey(currentArg) == false)
                {
                    if (indexes.Count == 0)
                    {
                        _orphanArgs.Add(currentArg);
                    }

                    continue;
                };

                indexes.Add(index);
            }

            _foundOptionLookup = indexes.ToArray();
        }

        private bool MoveToNextOption()
        {
            int nextIndex = LookupTheNextOptionIndex();
            if (nextIndex == -1) 
                return false;
            _currentOptionLookupIndex += 1;
            _currentOptionIndex = nextIndex;
            return true;
        }

        private int LookupTheNextOptionIndex()
        {
            return _foundOptionLookup.ElementAtOrDefault(_currentOptionLookupIndex + 1, -1);
        }

        private bool IsAKey(string arg)
        {
            return arg != null && _specialCharacters.OptionPrefix.Any(arg.StartsWith);
        }

        private bool IsACmd(string arg)
        {
            return arg != null && _specialCharacters.OptionPrefix.Any(arg.StartsWith) == false;
        }

        private bool IsEndOfOptionsKey(string arg)
        {
            return string.Equals(arg, _specialCharacters.EndOfOptionsKey, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
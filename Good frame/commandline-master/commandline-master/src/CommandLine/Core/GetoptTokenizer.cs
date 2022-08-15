using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;
using System.Text.RegularExpressions;

namespace CommandLine.Core
{
    static class GetoptTokenizer
    {
        public static Result<IEnumerable<Token>, Error> Tokenize(
            IEnumerable<string> arguments,
            Func<string, NameLookupResult> nameLookup)
        {
            return GetoptTokenizer.Tokenize(arguments, nameLookup, ignoreUnknownArguments: false, allowDashDash: true, posixlyCorrect: false);
        }

        public static Result<IEnumerable<Token>, Error> Tokenize(
            IEnumerable<string> arguments,
            Func<string, NameLookupResult> nameLookup,
            bool ignoreUnknownArguments,
            bool allowDashDash,
            bool posixlyCorrect)
        {
            List<Error> errors = new List<Error>();
            Action<string> onBadFormatToken = arg => errors.Add(new BadFormatTokenError(arg));
            Action<string> unknownOptionError = name => errors.Add(new UnknownOptionError(name));
            Action<string> doNothing = name => { };
            Action<string> onUnknownOption = ignoreUnknownArguments ? doNothing : unknownOptionError;

            int consumeNext = 0;
            Action<int> onConsumeNext = (n => consumeNext = consumeNext + n);
            bool forceValues = false;

            List<Token> tokens = new List<Token>();
            IEnumerator<string> enumerator = arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                switch (enumerator.Current)
                {
                    case null:
                        break;

                    case string arg when forceValues:
                        tokens.Add(Token.ValueForced(arg));
                        break;

                    case string arg when consumeNext > 0:
                        tokens.Add(Token.Value(arg));
                        consumeNext = consumeNext - 1;
                        break;

                    case "--" when allowDashDash:
                        forceValues = true;
                        break;

                    case "--":
                        tokens.Add(Token.Value("--"));
                        if (posixlyCorrect)
                            forceValues = true;
                        break;

                    case "-":
                        tokens.Add(Token.Value("-"));
                        if (posixlyCorrect)
                            forceValues = true;
                        break;

                    case string arg when arg.StartsWith("--"):
                        tokens.AddRange(TokenizeLongName(arg, nameLookup, onBadFormatToken, onUnknownOption, onConsumeNext));
                        break;

                    case string arg when arg.StartsWith("-"):
                        tokens.AddRange(TokenizeShortName(arg, nameLookup, onUnknownOption, onConsumeNext));
                        break;

                    case string arg:
                        tokens.Add(Token.Value(arg));
                        if (posixlyCorrect)
                            forceValues = true;
                        break;
                }
            }

            return Result.Succeed<IEnumerable<Token>, Error>(tokens.AsEnumerable(), errors.AsEnumerable());
        }

        public static Result<IEnumerable<Token>, Error> ExplodeOptionList(
            Result<IEnumerable<Token>, Error> tokenizerResult,
            Func<string, Maybe<char>> optionSequenceWithSeparatorLookup)
        {
            IEnumerable<Token> tokens = tokenizerResult.SucceededWith().Memoize();
            List<Token> exploded = new List<Token>(tokens is ICollection<Token> coll ? coll.Count : tokens.Count());
            Maybe<char> nothing = Maybe.Nothing<char>();
            Maybe<char> separator = nothing;
            foreach (var token in tokens)
            {
                if (token.IsName())
                {
                    separator = optionSequenceWithSeparatorLookup(token.Text);
                    exploded.Add(token);
                }
                else
                {
                    if (separator.MatchJust(out char sep) && sep != '\0' && !token.IsValueForced())
                    {
                        if (token.Text.Contains(sep))
                        {
                            exploded.AddRange(token.Text.Split(sep).Select(Token.ValueFromSeparator));
                        }
                        else
                        {
                            exploded.Add(token);
                        }
                    }
                    else
                    {
                        exploded.Add(token);
                    }
                    separator = nothing;  
                }
            }
            return Result.Succeed(exploded as IEnumerable<Token>, tokenizerResult.SuccessMessages());
        }

        public static Func<
                    IEnumerable<string>,
                    IEnumerable<OptionSpecification>,
                    Result<IEnumerable<Token>, Error>>
            ConfigureTokenizer(
                    StringComparer nameComparer,
                    bool ignoreUnknownArguments,
                    bool enableDashDash,
                    bool posixlyCorrect)
        {
            return (arguments, optionSpecs) =>
                {
                    var tokens = GetoptTokenizer.Tokenize(arguments, name => NameLookup.Contains(name, optionSpecs, nameComparer), ignoreUnknownArguments, enableDashDash, posixlyCorrect);
                    var explodedTokens = GetoptTokenizer.ExplodeOptionList(tokens, name => NameLookup.HavingSeparator(name, optionSpecs, nameComparer));
                    return explodedTokens;
                };
        }

        private static IEnumerable<Token> TokenizeShortName(
            string arg,
            Func<string, NameLookupResult> nameLookup,
            Action<string> onUnknownOption,
            Action<int> onConsumeNext)
        {
            string chars = arg.Substring(1);
            int len = chars.Length;
            if (len > 0 && Char.IsDigit(chars[0]))
            {
                yield return Token.Value(arg);
                yield break;
            }
            for (int i = 0; i < len; i++)
            {
                string s = new string(chars[i], 1);
                switch (nameLookup(s))
                {
                    case NameLookupResult.OtherOptionFound:
                        yield return Token.Name(s);

                        if (i + 1 < len)
                        {
                            yield return Token.Value(chars.Substring(i + 1));
                            yield break;
                        }
                        else
                        {
                            onConsumeNext(1);
                        }
                        break;

                    case NameLookupResult.NoOptionFound:
                        onUnknownOption(s);
                        break;

                    default:
                        yield return Token.Name(s);
                        break;
                }
            }
        }

        private static IEnumerable<Token> TokenizeLongName(
            string arg,
            Func<string, NameLookupResult> nameLookup,
            Action<string> onBadFormatToken,
            Action<string> onUnknownOption,
            Action<int> onConsumeNext)
        {
            string[] parts = arg.Substring(2).Split(new char[] { '=' }, 2);
            string name = parts[0];
            string value = (parts.Length > 1) ? parts[1] : null;
            if (string.IsNullOrWhiteSpace(name) || name.Contains(" "))
            {
                onBadFormatToken(arg);
                yield break;
            }
            switch (nameLookup(name))
            {
                case NameLookupResult.NoOptionFound:
                    onUnknownOption(name);
                    yield break;

                case NameLookupResult.OtherOptionFound:
                    yield return Token.Name(name);
                    if (value == null) 
                    {
                        onConsumeNext(1);
                    }
                    else
                    {
                        yield return Token.Value(value);
                    }
                    break;

                default:
                    yield return Token.Name(name);
                    break;
            }
        }
    }
}

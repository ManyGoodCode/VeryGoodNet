using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine.Core;
using CommandLine.Text;
using CSharpx;
using RailwaySharp.ErrorHandling;

namespace CommandLine
{
    public class Parser : IDisposable
    {
        private bool disposed;
        private readonly ParserSettings settings;
        private static readonly Lazy<Parser> DefaultParser = new Lazy<Parser>(
            () => new Parser(new ParserSettings { HelpWriter = Console.Error }));

        public Parser()
        {
            settings = new ParserSettings { Consumed = true };
        }

        public Parser(Action<ParserSettings> configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            settings = new ParserSettings();
            configuration(settings);
            settings.Consumed = true;
        }

        internal Parser(ParserSettings settings)
        {
            this.settings = settings;
            this.settings.Consumed = true;
        }

        ~Parser()
        {
            Dispose(false);
        }

        public static Parser Default
        {
            get { return DefaultParser.Value; }
        }

        public ParserSettings Settings
        {
            get { return settings; }
        }

        public ParserResult<T> ParseArguments<T>(IEnumerable<string> args)
        {
            if (args == null) throw new ArgumentNullException("args");

            var factory = typeof(T).IsMutable()
                ? Maybe.Just<Func<T>>(Activator.CreateInstance<T>)
                : Maybe.Nothing<Func<T>>();

            return MakeParserResult(
                InstanceBuilder.Build(
                    factory,
                    (arguments, optionSpecs) => Tokenize(arguments, optionSpecs, settings),
                    args,
                    settings.NameComparer,
                    settings.CaseInsensitiveEnumValues,
                    settings.ParsingCulture,
                    settings.AutoHelp,
                    settings.AutoVersion,
                    settings.AllowMultiInstance,
                    HandleUnknownArguments(settings.IgnoreUnknownArguments)),
                settings);
        }

        public ParserResult<T> ParseArguments<T>(Func<T> factory, IEnumerable<string> args)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (!typeof(T).IsMutable()) throw new ArgumentException("factory");
            if (args == null) throw new ArgumentNullException("args");

            return MakeParserResult(
                InstanceBuilder.Build(
                    Maybe.Just(factory),
                    (arguments, optionSpecs) => Tokenize(arguments, optionSpecs, settings),
                    args,
                    settings.NameComparer,
                    settings.CaseInsensitiveEnumValues,
                    settings.ParsingCulture,
                    settings.AutoHelp,
                    settings.AutoVersion,
                    settings.AllowMultiInstance,
                    HandleUnknownArguments(settings.IgnoreUnknownArguments)),
                settings);
        }

        public ParserResult<object> ParseArguments(IEnumerable<string> args, params Type[] types)
        {
            if (args == null) throw new ArgumentNullException("args");
            if (types == null) throw new ArgumentNullException("types");
            if (types.Length == 0) throw new ArgumentOutOfRangeException("types");

            return MakeParserResult(
                InstanceChooser.Choose(
                    (arguments, optionSpecs) => Tokenize(arguments, optionSpecs, settings),
                    types,
                    args,
                    settings.NameComparer,
                    settings.CaseInsensitiveEnumValues,
                    settings.ParsingCulture,
                    settings.AutoHelp,
                    settings.AutoVersion,
                    settings.AllowMultiInstance,
                    HandleUnknownArguments(settings.IgnoreUnknownArguments)),
                settings);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static Result<IEnumerable<Token>, Error> Tokenize(
                IEnumerable<string> arguments,
                IEnumerable<OptionSpecification> optionSpecs,
                ParserSettings settings)
        {
            return settings.GetoptMode
                ? GetoptTokenizer.ConfigureTokenizer(
                    settings.NameComparer,
                    settings.IgnoreUnknownArguments,
                    settings.EnableDashDash,
                    settings.PosixlyCorrect)(arguments, optionSpecs)
                : Tokenizer.ConfigureTokenizer(
                    settings.NameComparer,
                    settings.IgnoreUnknownArguments,
                    settings.EnableDashDash)(arguments, optionSpecs);
        }

        private static ParserResult<T> MakeParserResult<T>(ParserResult<T> parserResult, ParserSettings settings)
        {
            return DisplayHelp(
                parserResult,
                settings.HelpWriter,
                settings.MaximumDisplayWidth);
        }

        private static ParserResult<T> DisplayHelp<T>(ParserResult<T> parserResult, TextWriter helpWriter, int maxDisplayWidth)
        {
            parserResult.WithNotParsed(
                errors =>
                    Maybe.Merge(errors.ToMaybe(), helpWriter.ToMaybe())
                        .Do((_, writer) => writer.Write(HelpText.AutoBuild(parserResult, maxDisplayWidth)))
                );

            return parserResult;
        }

        private static IEnumerable<ErrorType> HandleUnknownArguments(bool ignoreUnknownArguments)
        {
            return ignoreUnknownArguments
                ? Enumerable.Empty<ErrorType>().Concat(ErrorType.UnknownOptionError)
                : Enumerable.Empty<ErrorType>();
        }

        private void Dispose(bool disposing)
        {
            if (disposed) 
                return;
            if (disposing)
            {
                if (settings != null)
                    settings.Dispose();

                disposed = true;
            }
        }
    }
}

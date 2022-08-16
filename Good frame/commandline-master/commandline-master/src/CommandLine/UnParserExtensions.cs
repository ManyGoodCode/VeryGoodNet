using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Core;
using CommandLine.Infrastructure;
using CSharpx;

namespace CommandLine
{
    public class UnParserSettings
    {
        private bool preferShortName;
        private bool groupSwitches;
        private bool useEqualToken;
        private bool showHidden;
        private bool skipDefault;

        public bool PreferShortName
        {
            get { return preferShortName; }
            set { PopsicleSetter.Set(Consumed, ref preferShortName, value); }
        }

        public bool GroupSwitches
        {
            get { return groupSwitches; }
            set { PopsicleSetter.Set(Consumed, ref groupSwitches, value); }
        }

        public bool UseEqualToken
        {
            get { return useEqualToken; }
            set { PopsicleSetter.Set(Consumed, ref useEqualToken, value); }
        }

        public bool ShowHidden
        {
            get { return showHidden; }
            set { PopsicleSetter.Set(Consumed, ref showHidden, value); }
        }

        public bool SkipDefault
        {
            get { return skipDefault; }
            set { PopsicleSetter.Set(Consumed, ref skipDefault, value); }
        }

        public static UnParserSettings WithGroupSwitchesOnly()
        {
            return new UnParserSettings { GroupSwitches = true };
        }


        public static UnParserSettings WithUseEqualTokenOnly()
        {
            return new UnParserSettings { UseEqualToken = true };
        }

        internal bool Consumed { get; set; }
    }

    public static class UnParserExtensions
    {
        public static string FormatCommandLine<T>(this Parser parser, T options)
        {
            return parser.FormatCommandLine(options, config => { });
        }

        public static string[] FormatCommandLineArgs<T>(this Parser parser, T options)
        {
            return parser.FormatCommandLine(options, config => { }).SplitArgs();
        }

        public static string FormatCommandLine<T>(this Parser parser, T options, Action<UnParserSettings> configuration)
        {
            if (options == null) 
                throw new ArgumentNullException("options");
            UnParserSettings settings = new UnParserSettings();
            configuration(settings);
            settings.Consumed = true;

            Type type = options.GetType();
            StringBuilder builder = new StringBuilder();

            type.GetVerbSpecification()
                .MapValueOrDefault(verb => builder.Append(verb.Name).Append(' '), builder);

            var specs =
                (from info in
                    type.GetSpecifications(
                        pi => new
                        {
                            Specification = Specification.FromProperty(pi),
                            Value = pi.GetValue(options, null).NormalizeValue(),
                            PropertyValue = pi.GetValue(options, null)
                        })
                 where !info.PropertyValue.IsEmpty(info.Specification, settings.SkipDefault)
                 select info)
                    .Memoize();

            var allOptSpecs = from info in specs.Where(i => i.Specification.Tag == SpecificationType.Option)
                              let o = (OptionSpecification)info.Specification
                              where o.TargetType != TargetType.Switch ||
                                   (o.TargetType == TargetType.Switch && o.FlagCounter && ((int)info.Value > 0)) ||
                                   (o.TargetType == TargetType.Switch && ((bool)info.Value))
                              where !o.Hidden || settings.ShowHidden
                              orderby o.UniqueName()
                              select info;

            var shortSwitches = from info in allOptSpecs
                                let o = (OptionSpecification)info.Specification
                                where o.TargetType == TargetType.Switch
                                where o.ShortName.Length > 0
                                orderby o.UniqueName()
                                select info;

            var optSpecs = settings.GroupSwitches
                ? allOptSpecs.Where(info => !shortSwitches.Contains(info))
                : allOptSpecs;

            var valSpecs = from info in specs.Where(i => i.Specification.Tag == SpecificationType.Value)
                           let v = (ValueSpecification)info.Specification
                           orderby v.Index
                           select info;

            builder = settings.GroupSwitches && shortSwitches.Any()
                ? builder.Append('-').Append(string.Join(string.Empty, shortSwitches.Select(
                    info => {
                        var o = (OptionSpecification)info.Specification;
                        return o.FlagCounter
                            ? string.Concat(Enumerable.Repeat(o.ShortName, (int)info.Value))
                            : o.ShortName;
                    }).ToArray())).Append(' ')
                : builder;
            optSpecs.ForEach(
                opt =>
                    builder
                        .Append(FormatOption((OptionSpecification)opt.Specification, opt.Value, settings))
                        .Append(' ')
                );

            builder.AppendWhen(valSpecs.Any() && parser.Settings.EnableDashDash, "-- ");

            valSpecs.ForEach(
                val => builder.Append(FormatValue(val.Specification, val.Value)).Append(' '));

            return builder
                .ToString().TrimEnd(' ');
        }

        public static string[] FormatCommandLineArgs<T>(this Parser parser, T options, Action<UnParserSettings> configuration)
        {
            return FormatCommandLine<T>(parser, options, configuration).SplitArgs();
        }

        private static string FormatValue(Specification spec, object value)
        {
            StringBuilder builder = new StringBuilder();
            switch (spec.TargetType)
            {
                case TargetType.Scalar:
                    builder.Append(FormatWithQuotesIfString(value));
                    break;
                case TargetType.Sequence:
                    var sep = spec.SeperatorOrSpace();
                    Func<object, object> format = v
                        => sep == ' ' ? FormatWithQuotesIfString(v) : v;
                    var e = ((IEnumerable)value).GetEnumerator();
                    while (e.MoveNext())
                        builder.Append(format(e.Current)).Append(sep);
                    builder.TrimEndIfMatch(sep);
                    break;
            }
            return builder.ToString();
        }

        private static object FormatWithQuotesIfString(object value)
        {
            string s = value.ToString();
            if (!string.IsNullOrEmpty(s) && !s.Contains("\"") && s.Contains(" "))
                return $"\"{s}\"";

            Func<string, string> doubQt = v
                => v.Contains("\"") ? v.Replace("\"", "\\\"") : v;

            return s.ToMaybe()
                    .MapValueOrDefault(v => v.Contains(' ') || v.Contains("\"")
                        ? "\"".JoinTo(doubQt(v), "\"") : v, value);
        }

        private static char SeperatorOrSpace(this Specification spec)
        {
            return (spec as OptionSpecification).ToMaybe()
                .MapValueOrDefault(o => o.Separator != '\0' ? o.Separator : ' ', ' ');
        }

        private static string FormatOption(OptionSpecification spec, object value, UnParserSettings settings)
        {
            return new StringBuilder()
                    .Append(spec.FormatName(value, settings))
                    .AppendWhen(spec.TargetType != TargetType.Switch, FormatValue(spec, value))
                .ToString();
        }

        private static string FormatName(this OptionSpecification optionSpec, object value, UnParserSettings settings)
        {
            var longName = (optionSpec.LongName.Length > 0 && !settings.PreferShortName)
                         || optionSpec.ShortName.Length == 0;

            var formattedName =
                new StringBuilder(longName
                    ? "--".JoinTo(optionSpec.LongName)
                    : "-".JoinTo(optionSpec.ShortName))
                        .AppendWhen(optionSpec.TargetType != TargetType.Switch, longName && settings.UseEqualToken ? "=" : " ")
                    .ToString();
            return optionSpec.FlagCounter ? String.Join(" ", Enumerable.Repeat(formattedName, (int)value)) : formattedName;
        }

        private static object NormalizeValue(this object value)
        {
            return value;
        }

        private static bool IsEmpty(this object value, Specification specification, bool skipDefault)
        {
            if (value == null) return true;

            if (skipDefault && value.Equals(specification.DefaultValue.FromJust())) return true;
            if (Nullable.GetUnderlyingType(specification.ConversionType) != null) return false; //nullable
            if (value is ValueType && value.Equals(value.GetType().GetDefaultValue())) return true;
            if (value is string && ((string)value).Length == 0) return true;
            if (value is IEnumerable && !((IEnumerable)value).GetEnumerator().MoveNext()) return true;
            return false;
        }

        public static string[] SplitArgs(this string command, bool keepQuote = false)
        {
            if (string.IsNullOrEmpty(command))
                return new string[0];

            var inQuote = false;
            var chars = command.ToCharArray().Select(v =>
            {
                if (v == '"')
                    inQuote = !inQuote;
                return !inQuote && v == ' ' ? '\n' : v;
            }).ToArray();

            return new string(chars).Split('\n')
                .Select(x => keepQuote ? x : x.Trim('"'))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
    }
}

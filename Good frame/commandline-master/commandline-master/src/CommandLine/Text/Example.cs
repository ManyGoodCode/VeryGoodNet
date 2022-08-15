using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Text
{
    public sealed class Example : IEquatable<Example>
    {
        private readonly string helpText;
        private readonly IEnumerable<UnParserSettings> formatStyles;
        private readonly object sample;

        public Example(string helpText, IEnumerable<UnParserSettings> formatStyles, object sample)
        {
            if (string.IsNullOrEmpty(helpText)) throw new ArgumentException("helpText can't be null or empty", "helpText");
            if (formatStyles == null) throw new ArgumentNullException("formatStyles");
            if (sample == null) throw new ArgumentNullException("sample");

            this.helpText = helpText;
            this.formatStyles = formatStyles;
            this.sample = sample;
        }

        public Example(string helpText, UnParserSettings formatStyle, object sample)
            : this(helpText, new[] { formatStyle }, sample)
        {
        }

        public Example(string helpText, object sample)
            : this(helpText, Enumerable.Empty<UnParserSettings>(), sample)
        {
        }

        public string HelpText
        {
            get { return helpText; }
        }
        
        public IEnumerable<UnParserSettings> FormatStyles
        {
            get { return this.formatStyles; }
        }

        public object Sample
        {
            get { return sample; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Example;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { HelpText, FormatStyles, Sample }.GetHashCode();
        }

        public bool Equals(Example other)
        {
            if (other == null)
            {
                return false;
            }

            return HelpText.Equals(other.HelpText)
                && FormatStyles.SequenceEqual(other.FormatStyles)
                && Sample.Equals(other.Sample);
        }
    }

    static class ExampleExtensions
    {
        public static IEnumerable<UnParserSettings> GetFormatStylesOrDefault(this Example example)
        {
            return example.FormatStyles.Any()
                ? example.FormatStyles
                : new[] { new UnParserSettings { Consumed = true } };
        }
    }
}

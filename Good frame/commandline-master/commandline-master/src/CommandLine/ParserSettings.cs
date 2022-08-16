using System;
using System.Globalization;
using System.IO;

using CommandLine.Infrastructure;
using CSharpx;

namespace CommandLine
{
    public class ParserSettings : IDisposable
    {
        private const int DefaultMaximumLength = 80; 

        private bool disposed;
        private bool caseSensitive;
        private bool caseInsensitiveEnumValues;
        private TextWriter helpWriter;
        private bool ignoreUnknownArguments;
        private bool autoHelp;
        private bool autoVersion;
        private CultureInfo parsingCulture;
        private Maybe<bool> enableDashDash;
        private int maximumDisplayWidth;
        private Maybe<bool> allowMultiInstance;
        private bool getoptMode;
        private Maybe<bool> posixlyCorrect;

        public ParserSettings()
        {
            caseSensitive = true;
            caseInsensitiveEnumValues = false;
            autoHelp = true;
            autoVersion = true;
            parsingCulture = CultureInfo.InvariantCulture;
            maximumDisplayWidth = GetWindowWidth();
            getoptMode = false;
            enableDashDash = Maybe.Nothing<bool>();
            allowMultiInstance = Maybe.Nothing<bool>();
            posixlyCorrect = Maybe.Nothing<bool>();
        }

        private int GetWindowWidth()
        {

#if !NET40
            if (Console.IsOutputRedirected) return DefaultMaximumLength;
#endif
            var width = 1;
            try
            {
                width = Console.WindowWidth;
                if (width < 1)
                {
                    width = DefaultMaximumLength;
                }
            }           
            catch (Exception e) when (e is IOException || e is PlatformNotSupportedException || e is ArgumentOutOfRangeException)
            {
               width = DefaultMaximumLength;
            }
            return width;
        }

        ~ParserSettings()
        {
            Dispose(false);
        }

        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { PopsicleSetter.Set(Consumed, ref caseSensitive, value); }
        }

        public bool CaseInsensitiveEnumValues
        {
            get { return caseInsensitiveEnumValues; }
            set { PopsicleSetter.Set(Consumed, ref caseInsensitiveEnumValues, value); }
        }

        public CultureInfo ParsingCulture
        {
            get { return parsingCulture; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                PopsicleSetter.Set(Consumed, ref parsingCulture, value); 
            }
        }

        public TextWriter HelpWriter
        {
            get { return helpWriter; }
            set { PopsicleSetter.Set(Consumed, ref helpWriter, value); }
        }

        public bool IgnoreUnknownArguments
        {
            get { return ignoreUnknownArguments; }
            set { PopsicleSetter.Set(Consumed, ref ignoreUnknownArguments, value); }
        }

        public bool AutoHelp
        {
            get { return autoHelp; }
            set { PopsicleSetter.Set(Consumed, ref autoHelp, value); }
        }

        public bool AutoVersion
        {
            get { return autoVersion; }
            set { PopsicleSetter.Set(Consumed, ref autoVersion, value); }
        }

        public bool EnableDashDash
        {
            get => enableDashDash.MatchJust(out bool value) ? value : getoptMode;
            set => PopsicleSetter.Set(Consumed, ref enableDashDash, Maybe.Just(value));
        }

        public int MaximumDisplayWidth
        {
            get { return maximumDisplayWidth; }
            set { maximumDisplayWidth = value; }
        }

        public bool AllowMultiInstance
        {
            get => allowMultiInstance.MatchJust(out bool value) ? value : getoptMode;
            set => PopsicleSetter.Set(Consumed, ref allowMultiInstance, Maybe.Just(value));
        }

        public bool GetoptMode
        {
            get => getoptMode;
            set => PopsicleSetter.Set(Consumed, ref getoptMode, value);
        }

        public bool PosixlyCorrect
        {
            get => posixlyCorrect.MapValueOrDefault(val => val, () => Environment.GetEnvironmentVariable("POSIXLY_CORRECT").ToBooleanLoose());
            set => PopsicleSetter.Set(Consumed, ref posixlyCorrect, Maybe.Just(value));
        }

        internal StringComparer NameComparer
        {
            get
            {
                return CaseSensitive
                    ? StringComparer.Ordinal
                    : StringComparer.OrdinalIgnoreCase;
            }
        }

        internal bool Consumed { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                disposed = true;
            }
        }
    }
}

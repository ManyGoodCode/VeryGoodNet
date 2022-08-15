using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using CommandLine.Infrastructure;
using CSharpx;

namespace CommandLine.Text
{
    public class CopyrightInfo
    {
        private const string DefaultCopyrightWord = "Copyright";
        private const string SymbolLower = "(c)";
        private const string SymbolUpper = "(C)";
        private readonly AssemblyCopyrightAttribute attribute;
        private readonly bool isSymbolUpper;
        private readonly int[] copyrightYears;
        private readonly string author;
        private readonly int builderSize;

        public static CopyrightInfo Empty
        {
            get
            {
                return new CopyrightInfo("author", DateTime.Now.Year);
            }
        }

        public CopyrightInfo(string author, int year)
            : this(true, author, new[] { year })
        {
        }

        public CopyrightInfo(string author, params int[] years)
            : this(true, author, years)
        {
        }

        public CopyrightInfo(bool isSymbolUpper, string author, params int[] copyrightYears)
        {
            if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("author");
            if (copyrightYears.Length == 0) throw new ArgumentOutOfRangeException("copyrightYears");

            const int ExtraLength = 10;
            this.isSymbolUpper = isSymbolUpper;
            this.author = author;
            this.copyrightYears = copyrightYears;
            builderSize = 12 + author.Length + (4 * copyrightYears.Length) + ExtraLength;
        }

        protected CopyrightInfo()
        {
        }

        private CopyrightInfo(AssemblyCopyrightAttribute attribute)
        {
            this.attribute = attribute;
        }

        public static CopyrightInfo Default
        {
            get
            {
                var copyrightAttr = ReflectionHelper.GetAttribute<AssemblyCopyrightAttribute>();
                switch (copyrightAttr.Tag)
                {
                    case MaybeType.Just:
                        return new CopyrightInfo(copyrightAttr.FromJustOrFail());
                    default:
                        var companyAttr = ReflectionHelper.GetAttribute<AssemblyCompanyAttribute>();
                        return companyAttr.IsNothing()
                            ? Empty
                            : new CopyrightInfo(companyAttr.FromJust().Company, DateTime.Now.Year);
                        
                }
            }
        }

        protected virtual string CopyrightWord
        {
            get { return DefaultCopyrightWord; }
        }


        public static implicit operator string(CopyrightInfo info)
        {
            return info.ToString();
        }

        public override string ToString()
        {
            if (attribute != null)
            {
                return attribute.Copyright;
            }

            return new StringBuilder(builderSize)
                .Append(CopyrightWord)
                .Append(' ')
                .Append(isSymbolUpper ? SymbolUpper : SymbolLower)
                .Append(' ')
                .Append(FormatYears(copyrightYears))
                .Append(' ')
                .Append(author)
                .ToString();
        }

        protected virtual string FormatYears(int[] years)
        {
            if (years.Length == 1)
            {
                return years[0].ToString(CultureInfo.InvariantCulture);
            }

            var yearsPart = new StringBuilder(years.Length * 6);
            for (var i = 0; i < years.Length; i++)
            {
                yearsPart.Append(years[i].ToString(CultureInfo.InvariantCulture));
                var next = i + 1;
                if (next < years.Length)
                {
                    yearsPart.Append(years[next] - years[i] > 1 ? " - " : ", ");
                }
            }

            return yearsPart.ToString();
        }
    }
}

using System;
using System.Text;
using System.Linq;

namespace CommandLine.Text
{
    public abstract class MultilineTextAttribute : Attribute
    {
        private readonly string line1;
        private readonly string line2;
        private readonly string line3;
        private readonly string line4;
        private readonly string line5;

        protected MultilineTextAttribute(string line1)
            : this(line1, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }


        protected MultilineTextAttribute(string line1, string line2)
            : this(line1, line2, string.Empty, string.Empty, string.Empty)
        {
        }

        protected MultilineTextAttribute(string line1, string line2, string line3)
            : this(line1, line2, line3, string.Empty, string.Empty)
        {
        }

        protected MultilineTextAttribute(string line1, string line2, string line3, string line4)
            : this(line1, line2, line3, line4, string.Empty)
        {
        }

        protected MultilineTextAttribute(string line1, string line2, string line3, string line4, string line5)
        {
            if (line1 == null) throw new ArgumentException("line1");
            if (line2 == null) throw new ArgumentException("line2");
            if (line3 == null) throw new ArgumentException("line3");
            if (line4 == null) throw new ArgumentException("line4");
            if (line5 == null) throw new ArgumentException("line5");
            
            this.line1 = line1;
            this.line2 = line2;
            this.line3 = line3;
            this.line4 = line4;
            this.line5 = line5;
        }

        public virtual string Value
        {
            get
            {
                var value = new StringBuilder(string.Empty);
                var strArray = new[] { line1, line2, line3, line4, line5 };

                for (var i = 0; i < GetLastLineWithText(strArray); i++)
                {
                    value.AppendLine(strArray[i]);
                }

                return value.ToString();
            }
        }

        public string Line1
        {
            get { return line1; }
        }

        public string Line2
        {
            get { return line2; }
        }

        public string Line3
        {
            get { return line3; }
        }

        public string Line4
        {
            get { return line4; }
        }

        public string Line5
        {
            get { return line5; }
        }

        internal HelpText AddToHelpText(HelpText helpText, Func<string, HelpText> func)
        {
            var strArray = new[] { line1, line2, line3, line4, line5 };
            return strArray.Take(GetLastLineWithText(strArray)).Aggregate(helpText, (current, line) => func(line));
        }

        internal HelpText AddToHelpText(HelpText helpText, bool before)
        {
            return before
                ? AddToHelpText(helpText, helpText.AddPreOptionsLine)
                : AddToHelpText(helpText, helpText.AddPostOptionsLine);
        }

        protected virtual int GetLastLineWithText(string[] value)
        {
            var index = Array.FindLastIndex(value, str => !string.IsNullOrEmpty(str));
            return index + 1;
        }
    }
}

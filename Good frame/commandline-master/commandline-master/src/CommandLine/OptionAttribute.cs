using CommandLine.Infrastructure;
using System;

namespace CommandLine
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : BaseAttribute
    {
        private readonly string longName;
        private readonly string shortName;
        private string setName;
        private bool flagCounter;
        private char separator;
        private string group=string.Empty;

        private OptionAttribute(string shortName, string longName) : base()
        {
            if (shortName == null) throw new ArgumentNullException("shortName");
            if (longName == null) throw new ArgumentNullException("longName");

            this.shortName = shortName;
            this.longName = longName;
            setName = string.Empty;
            separator = '\0';
        }

        public OptionAttribute()
            : this(string.Empty, string.Empty)
        {
        }

        public OptionAttribute(string longName)
            : this(string.Empty, longName)
        {
        }

        public OptionAttribute(char shortName, string longName)
            : this(shortName.ToOneCharString(), longName)
        {
        }

        public OptionAttribute(char shortName)
            : this(shortName.ToOneCharString(), string.Empty)
        {
        }

        public string LongName
        {
            get { return longName; }
        }

        public string ShortName
        {
            get { return shortName; }
        }

        public string SetName
        {
            get { return setName; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                setName = value;
            }
        }

        public bool FlagCounter
        {
            get { return flagCounter; }
            set { flagCounter = value; }
        }

        public char Separator
        {
            get { return separator; }
            set { separator = value; }
        }

        public string Group
        {
            get { return group; }
            set { group = value; }
        }
    }
}

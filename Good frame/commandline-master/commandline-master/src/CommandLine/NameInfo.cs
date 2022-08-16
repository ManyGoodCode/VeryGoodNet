using System;
using CommandLine.Core;

namespace CommandLine
{
    public sealed class NameInfo : IEquatable<NameInfo>
    {
        public static readonly NameInfo EmptyName = new NameInfo(string.Empty, string.Empty);
        private readonly string longName;
        private readonly string shortName;

        internal NameInfo(string shortName, string longName)
        {
            if (shortName == null) throw new ArgumentNullException("shortName");
            if (longName == null) throw new ArgumentNullException("longName");

            this.longName = longName;
            this.shortName = shortName;
        }

        public string ShortName
        {
            get { return shortName; }
        }

        public string LongName
        {
            get { return longName; }
        }

        public string NameText
        {
            get
            {
                return ShortName.Length > 0 && LongName.Length > 0
                           ? ShortName + ", " + LongName
                           : ShortName.Length > 0
                                ? ShortName
                                : LongName;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as NameInfo;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { ShortName, LongName }.GetHashCode();
        }

        public bool Equals(NameInfo other)
        {
            if (other == null)
            {
                return false;
            }

            return ShortName.Equals(other.ShortName) && LongName.Equals(other.LongName);
        }
    }
}

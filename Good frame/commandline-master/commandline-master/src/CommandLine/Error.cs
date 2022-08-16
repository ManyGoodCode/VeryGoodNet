using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine
{
    public enum ErrorType
    {
        BadFormatTokenError,
        MissingValueOptionError,
        UnknownOptionError,
        MissingRequiredOptionError,
        MutuallyExclusiveSetError,
        BadFormatConversionError,
        SequenceOutOfRangeError,
        RepeatedOptionError,
        NoVerbSelectedError,
        BadVerbSelectedError,
        HelpRequestedError,
        HelpVerbRequestedError,
        VersionRequestedError,
        SetValueExceptionError,
        InvalidAttributeConfigurationError,
        MissingGroupOptionError,
        GroupOptionAmbiguityError, 
        MultipleDefaultVerbsError

    }

    public abstract class Error : IEquatable<Error>
    {
        private readonly ErrorType tag;
        private readonly bool stopsProcessing;
        protected internal Error(ErrorType tag, bool stopsProcessing)
        {
            this.tag = tag;
            this.stopsProcessing = stopsProcessing;
        }

        protected internal Error(ErrorType tag)
            : this(tag, false)
        {
        }

        public ErrorType Tag
        {
            get { return tag; }
        }

        public bool StopsProcessing
        {
            get { return stopsProcessing; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Error;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { Tag, StopsProcessing }.GetHashCode();
        }

        public bool Equals(Error other)
        {
            if (other == null)
            {
                return false;
            }

            return Tag.Equals(other.Tag);
        }
    }

    public abstract class TokenError : Error, IEquatable<TokenError>
    {
        private readonly string token;
        protected internal TokenError(ErrorType tag, string token)
            : base(tag)
        {
            if (token == null) throw new ArgumentNullException("token");

            this.token = token;
        }

        public string Token
        {
            get { return token; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as TokenError;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { Tag, StopsProcessing, Token }.GetHashCode();
        }

        public bool Equals(TokenError other)
        {
            if (other == null)
            {
                return false;
            }

            return Tag.Equals(other.Tag) && Token.Equals(other.Token);
        }
    }

    public sealed class BadFormatTokenError : TokenError
    {
        internal BadFormatTokenError(string token)
            : base(ErrorType.BadFormatTokenError, token)
        {
        }
    }

    public abstract class NamedError : Error, IEquatable<NamedError>
    {
        private readonly NameInfo nameInfo;

        protected internal NamedError(ErrorType tag, NameInfo nameInfo)
            : base(tag)
        {
            this.nameInfo = nameInfo;
        }

        public NameInfo NameInfo
        {
            get { return nameInfo; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as NamedError;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { Tag, StopsProcessing, NameInfo }.GetHashCode();
        }

        public bool Equals(NamedError other)
        {
            if (other == null)
            {
                return false;
            }

            return Tag.Equals(other.Tag) && NameInfo.Equals(other.NameInfo);
        }
    }

    public sealed class MissingValueOptionError : NamedError
    {
        internal MissingValueOptionError(NameInfo nameInfo)
            : base(ErrorType.MissingValueOptionError, nameInfo)
        {
        }
    }

    public sealed class UnknownOptionError : TokenError
    {
        internal UnknownOptionError(string token)
            : base(ErrorType.UnknownOptionError, token)
        {
        }
    }

    public sealed class MissingRequiredOptionError : NamedError
    {
        internal MissingRequiredOptionError(NameInfo nameInfo)
            : base(ErrorType.MissingRequiredOptionError, nameInfo)
        {
        }
    }

    public sealed class MutuallyExclusiveSetError : NamedError
    {
        private readonly string setName;

        internal MutuallyExclusiveSetError(NameInfo nameInfo, string setName)
            : base(ErrorType.MutuallyExclusiveSetError, nameInfo)
        {
            this.setName = setName;
        }

        public string SetName
        {
            get { return setName; }
        }
    }

    public sealed class BadFormatConversionError : NamedError
    {
        internal BadFormatConversionError(NameInfo nameInfo)
            : base(ErrorType.BadFormatConversionError, nameInfo)
        {
        }
    }

    public sealed class SequenceOutOfRangeError : NamedError
    {
        internal SequenceOutOfRangeError(NameInfo nameInfo)
            : base(ErrorType.SequenceOutOfRangeError, nameInfo)
        {
        }
    }

    public sealed class RepeatedOptionError : NamedError
    {
        internal RepeatedOptionError(NameInfo nameInfo)
            : base(ErrorType.RepeatedOptionError, nameInfo)
        {
        }
    }

    public sealed class BadVerbSelectedError : TokenError
    {
        internal BadVerbSelectedError(string token)
            : base(ErrorType.BadVerbSelectedError, token)
        {
        }
    }

    public sealed class HelpRequestedError : Error
    {
        internal HelpRequestedError()
            : base(ErrorType.HelpRequestedError, true)
        {
        }
    }

    public sealed class HelpVerbRequestedError : Error
    {
        private readonly string verb;
        private readonly Type type;
        private readonly bool matched;

        internal HelpVerbRequestedError(string verb, Type type, bool matched)
            : base(ErrorType.HelpVerbRequestedError, true)
        {
            this.verb = verb;
            this.type = type;
            this.matched = matched;
        }

        public string Verb
        {
            get { return verb; }
        }

        public Type Type
        {
            get { return type; }
        }

        public bool Matched
        {
            get { return matched; }
        }
    }

    public sealed class NoVerbSelectedError : Error
    {
        internal NoVerbSelectedError()
            : base(ErrorType.NoVerbSelectedError)
        {
        }
    }

    public sealed class VersionRequestedError : Error
    {
        internal VersionRequestedError()
            : base(ErrorType.VersionRequestedError, true)
        {
        }
    }

    public sealed class SetValueExceptionError : NamedError
    {
        private readonly Exception exception;
        private readonly object value;

        internal SetValueExceptionError(NameInfo nameInfo, Exception exception, object value)
            : base(ErrorType.SetValueExceptionError, nameInfo)
        {
            this.exception = exception;
            this.value = value;
        }

        public Exception Exception
        {
            get { return exception; }
        }

        public object Value
        {
            get { return value; }
        }
    }

    public sealed class InvalidAttributeConfigurationError : Error
    {
        public const string ErrorMessage = "Check if Option or Value attribute values are set properly for the given type.";

        internal InvalidAttributeConfigurationError()
            : base(ErrorType.InvalidAttributeConfigurationError, true)
        {
        }
    }

    public sealed class MissingGroupOptionError : Error, IEquatable<Error>, IEquatable<MissingGroupOptionError>
    {
        public const string ErrorMessage = "At least one option in a group must have value.";

        private readonly string group;
        private readonly IEnumerable<NameInfo> names;

        internal MissingGroupOptionError(string group, IEnumerable<NameInfo> names)
            : base(ErrorType.MissingGroupOptionError)
        {
            this.group = group;
            this.names = names;
        }

        public string Group
        {
            get { return group; }
        }

        public IEnumerable<NameInfo> Names
        {
            get { return names; }
        }

        public new bool Equals(Error obj)
        {
            var other = obj as MissingGroupOptionError;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public bool Equals(MissingGroupOptionError other)
        {
            if (other == null)
            {
                return false;
            }

            return Group.Equals(other.Group) && Names.SequenceEqual(other.Names);
        }
    }

    public sealed class GroupOptionAmbiguityError : NamedError
    {
        public NameInfo Option;

        internal GroupOptionAmbiguityError(NameInfo option)
            : base(ErrorType.GroupOptionAmbiguityError, option)
        {
            Option = option;
        }
    }

    public sealed class MultipleDefaultVerbsError : Error
    {
        public const string ErrorMessage = "More than one default verb is not allowed.";

        internal MultipleDefaultVerbsError()
            : base(ErrorType.MultipleDefaultVerbsError)
        { }
    }
}

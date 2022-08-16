using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine
{
    public sealed class TypeInfo
    {
        private readonly Type current;
        private readonly IEnumerable<Type> choices;

        private TypeInfo(Type current, IEnumerable<Type> choices)
        {
            this.current = current;
            this.choices = choices;
        }

        public Type Current
        {
            get { return this.current; }
        }

        public IEnumerable<Type> Choices
        {
            get { return this.choices; }
        }

        internal static TypeInfo Create(Type current)
        {
            return new TypeInfo(current, Enumerable.Empty<Type>());
        }

        internal static TypeInfo Create(Type current, IEnumerable<Type> choices)
        {
            return new TypeInfo(current, choices);
        }
    }

    public enum ParserResultType
    {
        Parsed,
        NotParsed
    }

    public abstract class ParserResult<T>
    {
        private readonly ParserResultType tag;
        private readonly TypeInfo typeInfo;

        internal ParserResult(IEnumerable<Error> errors, TypeInfo typeInfo)
        {
            this.tag = ParserResultType.NotParsed;
            this.typeInfo = typeInfo ?? TypeInfo.Create(typeof(T));
            Errors = errors ?? new Error[0];
            Value = default;
        }

        internal ParserResult(T value, TypeInfo typeInfo)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            this.tag = ParserResultType.Parsed;
            this.typeInfo = typeInfo ?? TypeInfo.Create(value.GetType());
            Errors = new Error[0];
        }

        public ParserResultType Tag
        {
            get { return this.tag; }
        }

        public TypeInfo TypeInfo
        {
            get { return typeInfo; }
        }

        public T Value { get; }

        public IEnumerable<Error> Errors { get; }
    }

    public sealed class Parsed<T> : ParserResult<T>, IEquatable<Parsed<T>>
    {
        internal Parsed(T value, TypeInfo typeInfo)
            : base(value, typeInfo)
        {
        }

        internal Parsed(T value)
            : this(value, TypeInfo.Create(value.GetType()))
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is Parsed<T> other)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { Tag, Value }.GetHashCode();
        }

        public bool Equals(Parsed<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Tag.Equals(other.Tag)
                && Value.Equals(other.Value);
        }
    }

    public sealed class NotParsed<T> : ParserResult<T>, IEquatable<NotParsed<T>>
    {

        internal NotParsed(TypeInfo typeInfo, IEnumerable<Error> errors)
            : base(errors, typeInfo)
        {
        }


        public override bool Equals(object obj)
        {
            if (obj is NotParsed<T> other)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { Tag, Errors }.GetHashCode();
        }

        public bool Equals(NotParsed<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Tag.Equals(other.Tag)
                && Errors.SequenceEqual(other.Errors);
        }
    }
}

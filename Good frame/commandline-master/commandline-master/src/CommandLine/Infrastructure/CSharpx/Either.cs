using System;

namespace CSharpx
{
    enum EitherType
    {
        Left,
        Right
    }

    abstract class Either<TLeft, TRight>
    {
        private readonly EitherType tag;

        protected Either(EitherType tag)
        {
            this.tag = tag;
        }

        public EitherType Tag
        {
            get { return this.tag; }
        }

        public bool MatchLeft(out TLeft value)
        {
            value = Tag == EitherType.Left ? ((Left<TLeft, TRight>)this).Value : default(TLeft);
            return Tag == EitherType.Left;
        }

        public bool MatchRight(out TRight value)
        {
            value = Tag == EitherType.Right ? ((Right<TLeft, TRight>)this).Value : default(TRight);
            return Tag == EitherType.Right;
        }
    }

    sealed class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        private readonly TLeft value;

        internal Left(TLeft value)
            : base(EitherType.Left)
        {
            this.value = value;
        }

        public TLeft Value
        {
            get { return value; }
        }
    }

    sealed class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        private readonly TRight value;

        internal Right(TRight value)
            : base(EitherType.Right)
        {
            this.value = value;
        }

        public TRight Value
        {
            get { return value; }
        }
    }

    static class Either
    {
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value)
        {
            return new Left<TLeft, TRight>(value);
        }

        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value)
        {
            return new Right<TLeft, TRight>(value);
        }

        public static Either<string, TRight> Return<TRight>(TRight value)
        {
            return Either.Right<string, TRight>(value);
        }

        public static Either<string, TRight> Fail<TRight>(string message)
        {
            throw new Exception(message);
        }

        public static Either<TLeft, TResult> Bind<TLeft, TRight, TResult>(Either<TLeft, TRight> either, Func<TRight, Either<TLeft, TResult>> func)
        {
            TRight right;
            if (either.MatchRight(out right)) {
                return func(right);
            }
            return Either.Left<TLeft, TResult>(either.GetLeft());
        }

        public static Either<TLeft, TResult> Map<TLeft, TRight, TResult>(Either<TLeft, TRight> either, Func<TRight, TResult> func)
        {
            TRight right;
            if (either.MatchRight(out right)) {
                return Either.Right<TLeft, TResult>(func(right));
            }
            return Either.Left<TLeft, TResult>(either.GetLeft());
        }

        public static Either<TLeft1, TRight1> Bimap<TLeft, TRight, TLeft1, TRight1>(Either<TLeft, TRight> either, Func<TLeft, TLeft1> mapLeft, Func<TRight, TRight1> mapRight)
        {
            TRight right;
            if (either.MatchRight(out right)) {
                return Either.Right<TLeft1, TRight1>(mapRight(right));
            }
            return Either.Left<TLeft1, TRight1>(mapLeft(either.GetLeft()));
        }

        public static Either<TLeft, TResult> Select<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, TResult> selector)
        {
            return Either.Map(either, selector);
        }

        public static Either<TLeft, TResult> SelectMany<TLeft, TRight, TResult>(this Either<TLeft, TRight> result,
            Func<TRight, Either<TLeft, TResult>> func)
        {
            return Either.Bind(result, func);
        }

        public static TRight GetOrFail<TLeft, TRight>(Either<TLeft, TRight> either)
        {
            TRight value;
            if (either.MatchRight(out value)) {
                return value;
            }
            throw new ArgumentException(nameof(either), string.Format("The either value was Left {0}", either));
        }

        public static TLeft GetLeftOrDefault<TLeft, TRight>(Either<TLeft, TRight> either, TLeft @default)
        {
            TLeft value;
            return either.MatchLeft(out value) ? value : @default;
        }

        public static TRight GetRightOrDefault<TLeft, TRight>(Either<TLeft, TRight> either, TRight @default)
        {
            TRight value;
            return either.MatchRight(out value) ? value : @default;
        }

        public static Either<Exception, TRight> Try<TRight>(Func<TRight> func)
        {
            try {
                return new Right<Exception, TRight>(func());
            }
            catch (Exception ex) {
                return new Left<Exception, TRight>(ex);
            }
        }

        public static Either<Exception, TRight> Cast<TRight>(object obj)
        {
            return Either.Try(() => (TRight)obj);
        }

        public static Either<TLeft, TRight> FromMaybe<TLeft, TRight>(Maybe<TRight> maybe, TLeft left)
        {
            if (maybe.Tag == MaybeType.Just) {
                return Either.Right<TLeft, TRight>(((Just<TRight>)maybe).Value);
            }
            return Either.Left<TLeft, TRight>(left);
        }

        private static TLeft GetLeft<TLeft, TRight>(this Either<TLeft, TRight> either)
        {
            return ((Left<TLeft, TRight>)either).Value;
        }
    }

    static class EitherExtensions
    {
        public static void Match<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> ifLeft, Action<TRight> ifRight)
        {
            TLeft left;
            if (either.MatchLeft(out left)) {
                ifLeft(left);
                return;
            }

            ifRight(((Right<TLeft, TRight>)either).Value);
        }

        public static Either<string, TRight> ToEither<TRight>(this TRight value)
        {
            return Either.Return<TRight>(value);
        }

        public static Either<TLeft, TResult> Bind<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TResult>> func)
        {
            return Either.Bind(either, func);
        }

        public static Either<TLeft, TResult> Map<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, TResult> func)
        {
            return Either.Map(either, func);
        }

        public static Either<TLeft1, TRight1> Bimap<TLeft, TRight, TLeft1, TRight1>(
            this Either<TLeft, TRight> either,
            Func<TLeft, TLeft1> mapLeft,
            Func<TRight, TRight1> mapRight)
        {
            return Either.Bimap(either, mapLeft, mapRight);
        }

        public static bool IsLeft<TLeft, TRight>(this Either<TLeft, TRight> either)
        {
            return either.Tag == EitherType.Left;
        }

        public static bool IsRight<TLeft, TRight>(this Either<TLeft, TRight> either)
        {
            return either.Tag == EitherType.Right;
        }
    }
}

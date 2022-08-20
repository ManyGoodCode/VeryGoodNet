using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpx
{
    enum MaybeType
    {
        Just,
        Nothing
    }

    abstract class Maybe<T>
    {
        private readonly MaybeType tag;
        protected Maybe(MaybeType tag)
        {
            this.tag = tag;
        }

        public MaybeType Tag { get { return tag; } }

        /// <summary>
        /// 判断 是否为  MaybeType.Just 类型
        /// </summary>
        public bool MatchJust(out T value)
        {
            value = Tag == MaybeType.Just ? ((Just<T>)this).Value : default(T);
            return Tag == MaybeType.Just;
        }

        public bool MatchNothing()
        {
            return Tag == MaybeType.Nothing;
        }
    }

    sealed class Nothing<T> : Maybe<T>
    {
        internal Nothing()
            : base(MaybeType.Nothing)
        {
        }
    }

    sealed class Just<T> : Maybe<T>
    {
        private readonly T value;

        internal Just(T value)
            : base(MaybeType.Just)
        {
            this.value = value;
        }

        public T Value
        {
            get { return value; }
        }
    }

    static class Maybe
    {
        public static Maybe<T> Nothing<T>()
        {
            return new Nothing<T>();
        }

        public static Just<T> Just<T>(T value)
        {
            return new Just<T>(value);
        }

        public static Maybe<T> Return<T>(T value)
        {
            return Equals(value, default(T)) ? Maybe.Nothing<T>() : Maybe.Just(value);
        }

        public static Maybe<T2> Bind<T1, T2>(Maybe<T1> maybe, Func<T1, Maybe<T2>> func)
        {
            T1 value1;
            return maybe.MatchJust(out value1) ? func(value1) : Maybe.Nothing<T2>();
        }

        public static Maybe<T2> Map<T1, T2>(Maybe<T1> maybe, Func<T1, T2> func)
        {
            T1 value1;
            return maybe.MatchJust(out value1) ? Maybe.Just(func(value1)) : Maybe.Nothing<T2>();
        }

        public static Maybe<Tuple<T1, T2>> Merge<T1, T2>(Maybe<T1> first, Maybe<T2> second)
        {
            T1 value1;
            T2 value2;
            if (first.MatchJust(out value1) && second.MatchJust(out value2))
            {
                return Maybe.Just(Tuple.Create(value1, value2));
            }
            return Maybe.Nothing<Tuple<T1, T2>>();
        }
    }

    static class MaybeExtensions
    {
        public static void Match<T>(this Maybe<T> maybe, Action<T> ifJust, Action ifNothing)
        {
            T value;
            if (maybe.MatchJust(out value))
            {
                ifJust(value);
                return;
            }

            ifNothing();
        }

        public static void Match<T1, T2>(this Maybe<Tuple<T1, T2>> maybe, Action<T1, T2> ifJust, Action ifNothing)
        {
            T1 value1;
            T2 value2;
            if (maybe.MatchJust(out value1, out value2))
            {
                ifJust(value1, value2);
                return;
            }

            ifNothing();
        }

        public static bool MatchJust<T1, T2>(this Maybe<Tuple<T1, T2>> maybe, out T1 value1, out T2 value2)
        {
            Tuple<T1, T2> value;
            if (maybe.MatchJust(out value))
            {
                value1 = value.Item1;
                value2 = value.Item2;
                return true;
            }
            value1 = default(T1);
            value2 = default(T2);
            return false;
        }

        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return Maybe.Return(value);
        }

        public static Maybe<T2> Bind<T1, T2>(this Maybe<T1> maybe, Func<T1, Maybe<T2>> func)
        {
            return Maybe.Bind(maybe, func);
        }

        public static Maybe<T2> Map<T1, T2>(this Maybe<T1> maybe, Func<T1, T2> func)
        {
            return Maybe.Map(maybe, func);
        }

        public static Maybe<TResult> Select<TSource, TResult>(
            this Maybe<TSource> maybe,
            Func<TSource, TResult> selector)
        {
            return Maybe.Map(maybe, selector);
        }

        public static Maybe<TResult> SelectMany<TSource, TValue, TResult>(
            this Maybe<TSource> maybe,
            Func<TSource, Maybe<TValue>> valueSelector,
            Func<TSource, TValue, TResult> resultSelector)
        {
            return maybe
                .Bind(sourceValue =>
                        valueSelector(sourceValue)
                            .Map(resultValue => resultSelector(sourceValue, resultValue)));
        }

        public static void Do<T>(this Maybe<T> maybe, Action<T> action)
        {
            T value;
            if (maybe.MatchJust(out value))
            {
                action(value);
            }
        }

        public static void Do<T1, T2>(this Maybe<Tuple<T1, T2>> maybe, Action<T1, T2> action)
        {
            T1 value1;
            T2 value2;
            if (maybe.MatchJust(out value1, out value2))
            {
                action(value1, value2);
            }
        }

        public static bool IsJust<T>(this Maybe<T> maybe)
        {
            return maybe.Tag == MaybeType.Just;
        }

        public static bool IsNothing<T>(this Maybe<T> maybe)
        {
            return maybe.Tag == MaybeType.Nothing;
        }

        public static T FromJust<T>(this Maybe<T> maybe)
        {
            T value;
            if (maybe.MatchJust(out value))
            {
                return value;
            }
            return default(T);
        }

        public static T FromJustOrFail<T>(this Maybe<T> maybe, Exception exceptionToThrow = null)
        {
            T value;
            if (maybe.MatchJust(out value))
            {
                return value;
            }
            throw exceptionToThrow ?? new ArgumentException("Value empty.");
        }

        public static T GetValueOrDefault<T>(this Maybe<T> maybe, T noneValue)
        {
            T value;
            return maybe.MatchJust(out value) ? value : noneValue;
        }

        /// <summary>
        /// 如果有数据执行利用 Maybe 里面的值执行 func ，如果没有数据返回默认值 noneValue
        /// </summary>
        public static T2 MapValueOrDefault<T1, T2>(this Maybe<T1> maybe, Func<T1, T2> func, T2 noneValue)
        {
            T1 value1;
            return maybe.MatchJust(out value1) ? func(value1) : noneValue;
        }

        public static T2 MapValueOrDefault<T1, T2>(this Maybe<T1> maybe, Func<T1, T2> func, Func<T2> noneValueFactory)
        {
            T1 value1;
            return maybe.MatchJust(out value1) ? func(value1) : noneValueFactory();
        }

        public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
        {
            T value;
            if (maybe.MatchJust(out value))
            {
                return Enumerable.Empty<T>().Concat(new[] { value });
            }

            return Enumerable.Empty<T>();
        }
    }
}

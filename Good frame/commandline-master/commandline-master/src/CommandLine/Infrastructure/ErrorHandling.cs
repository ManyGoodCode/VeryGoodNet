using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace RailwaySharp.ErrorHandling
{
    enum ResultType
    {
        Ok,
        Bad
    }

    abstract class Result<TSuccess, TMessage>
    {
        private readonly ResultType _tag;

        protected Result(ResultType tag)
        {
            _tag = tag;
        }

        public ResultType Tag
        {
            get { return _tag; }
        }

        public override string ToString()
        {
            switch (Tag)
            {
                default:
                    var ok = (Ok<TSuccess, TMessage>)this;
                    return string.Format(
                        "OK: {0} - {1}",
                        ok.Success,
                        string.Join(Environment.NewLine, ok.Messages.Select(v => v.ToString())));
                case ResultType.Bad:
                    var bad = (Bad<TSuccess, TMessage>)this;
                    return string.Format(
                        "Error: {0}",
                        string.Join(Environment.NewLine, bad.Messages.Select(v => v.ToString())));
            }
        }
    }

    sealed class Ok<TSuccess, TMessage> : Result<TSuccess, TMessage>
    {
        private readonly Tuple<TSuccess, IEnumerable<TMessage>> _value;

        public Ok(TSuccess success, IEnumerable<TMessage> messages)
            : base(ResultType.Ok)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));

            _value = Tuple.Create(success, messages);
        }

        public TSuccess Success
        {
            get { return _value.Item1; }
        }

        public IEnumerable<TMessage> Messages
        {
            get { return _value.Item2; }
        }
    }

    sealed class Bad<TSuccess, TMessage> : Result<TSuccess, TMessage>
    {
        private readonly IEnumerable<TMessage> _messages;
        public Bad(IEnumerable<TMessage> messages)
            : base(ResultType.Bad)
        {
            if (messages == null) throw new ArgumentException(nameof(messages));

            _messages = messages;
        }

        public IEnumerable<TMessage> Messages
        {
            get { return _messages; }
        }
    }

    static class Result
    {
        public static Result<TSuccess, TMessage> FailWith<TSuccess, TMessage>(IEnumerable<TMessage> messages)
        {
            if (messages == null) throw new ArgumentException(nameof(messages));

            return new Bad<TSuccess, TMessage>(messages);
        }

        public static Result<TSuccess, TMessage> FailWith<TSuccess, TMessage>(TMessage message)
        {
            if (message == null) throw new ArgumentException(nameof(message));

            return new Bad<TSuccess, TMessage>(new[] { message });
        }

        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>());
        }

        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value, TMessage message)
        {
            if (message == null) throw new ArgumentException(nameof(message));

            return new Ok<TSuccess, TMessage>(value, new[] { message });
        }

        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value, IEnumerable<TMessage> messages)
        {
            if (messages == null) throw new ArgumentException(nameof(messages));

            return new Ok<TSuccess, TMessage>(value, messages);
        }

        public static Result<TSuccess, Exception> Try<TSuccess>(Func<TSuccess> func)
        {
            if (func == null) throw new ArgumentException(nameof(func));

            try
            {
                return new Ok<TSuccess, Exception>(
                        func(), Enumerable.Empty<Exception>());
            }
            catch (Exception ex)
            {
                return new Bad<TSuccess, Exception>(
                    new[] { ex });
            }
        }
    }

    static class Trial
    {
        public static Result<TSuccess, TMessage> Ok<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>());
        }

        public static Result<TSuccess, TMessage> Pass<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>());
        }

        public static Result<TSuccess, TMessage> Warn<TSuccess, TMessage>(TMessage message, TSuccess value)
        {
            if (message == null) throw new ArgumentException(nameof(message));

            return new Ok<TSuccess, TMessage>(value, new[] { message });
        }

        public static Result<TSuccess, TMessage> Fail<TSuccess, TMessage>(TMessage message)
        {
            if (message == null) throw new ArgumentException(nameof(message));

            return new Bad<TSuccess, TMessage>(new[] { message });
        }

        public static bool Failed<TSuccess, TMessage>(Result<TSuccess, TMessage> result)
        {
            return result.Tag == ResultType.Bad;
        }

        public static TResult Either<TSuccess, TMessage, TResult>(
            Func<TSuccess, IEnumerable<TMessage>, TResult> successFunc,
            Func<IEnumerable<TMessage>, TResult> failureFunc,
            Result<TSuccess, TMessage> trialResult)
        {
            if (successFunc == null) throw new ArgumentException(nameof(successFunc));
            if (failureFunc == null) throw new ArgumentException(nameof(failureFunc));

            var ok = trialResult as Ok<TSuccess, TMessage>;
            if (ok != null)
            {
                return successFunc(ok.Success, ok.Messages);
            }
            var bad = (Bad<TSuccess, TMessage>)trialResult;
            return failureFunc(bad.Messages);
        }

        public static TSuccess ReturnOrFail<TSuccess, TMessage>(Result<TSuccess, TMessage> result)
        {
            Func<IEnumerable<TMessage>, TSuccess> raiseExn = msgs =>
            {
                throw new Exception(
                    string.Join(
                    Environment.NewLine, msgs.Select(m => m.ToString())));
            };

            return Either((succ, _) => succ, raiseExn, result);
        }

        public static Result<TSuccess, TMessage> MergeMessages<TSuccess, TMessage>(
            IEnumerable<TMessage> messages,
            Result<TSuccess, TMessage> result)
        {
            if (messages == null) throw new ArgumentException(nameof(messages));

            Func<TSuccess, IEnumerable<TMessage>, Result<TSuccess, TMessage>> successFunc =
                (succ, msgs) =>
                    new Ok<TSuccess, TMessage>(
                        succ, messages.Concat(msgs));

            Func<IEnumerable<TMessage>, Result<TSuccess, TMessage>> failureFunc =
                errors => new Bad<TSuccess, TMessage>(errors.Concat(messages));

            return Either(successFunc, failureFunc, result);
        }

        public static Result<TSuccess, TMessage> Bind<TValue, TSuccess, TMessage>(
            Func<TValue, Result<TSuccess, TMessage>> func,
            Result<TValue, TMessage> result)
        {
            if (func == null) throw new ArgumentException(nameof(func));

            Func<TValue, IEnumerable<TMessage>, Result<TSuccess, TMessage>> successFunc =
                (succ, msgs) => MergeMessages(msgs, func(succ));

            Func<IEnumerable<TMessage>, Result<TSuccess, TMessage>> failureFunc =
                messages => new Bad<TSuccess, TMessage>(messages);

            return Either(successFunc, failureFunc, result);
        }

        public static Result<TSuccess, TMessage> Flatten<TSuccess, TMessage>(
            Result<Result<TSuccess, TMessage>, TMessage> result)
        {
            return Bind(x => x, result);
        }

        public static Result<TSuccess, TMessage> Apply<TValue, TSuccess, TMessage>(
            Result<Func<TValue, TSuccess>, TMessage> wrappedFunction,
            Result<TValue, TMessage> result)
        {
            if (wrappedFunction == null) throw new ArgumentException(nameof(wrappedFunction));

            if (wrappedFunction.Tag == ResultType.Ok && result.Tag == ResultType.Ok)
            {
                var ok1 = (Ok<Func<TValue, TSuccess>, TMessage>)wrappedFunction;
                var ok2 = (Ok<TValue, TMessage>)result;

                return new Ok<TSuccess, TMessage>(
                    ok1.Success(ok2.Success), ok1.Messages.Concat(ok2.Messages));
            }
            if (wrappedFunction.Tag == ResultType.Bad && result.Tag == ResultType.Ok)
            {
                return new Bad<TSuccess, TMessage>(((Bad<TValue, TMessage>)result).Messages);
            }
            if (wrappedFunction.Tag == ResultType.Ok && result.Tag == ResultType.Bad)
            {
                return new Bad<TSuccess, TMessage>(
                    ((Bad<TValue, TMessage>)result).Messages);
            }

            var bad1 = (Bad<Func<TValue, TSuccess>, TMessage>)wrappedFunction;
            var bad2 = (Bad<TValue, TMessage>)result;
            return new Bad<TSuccess, TMessage>(bad1.Messages.Concat(bad2.Messages));
        }


        public static Result<TSuccess, TMessage> Lift<TValue, TSuccess, TMessage>(
            Func<TValue, TSuccess> func,
            Result<TValue, TMessage> result)
        {
            return Apply(Ok<Func<TValue, TSuccess>, TMessage>(func), result);
        }

        public static Result<TSuccess1, TMessage1> Lift2<TSuccess, TMessage, TSuccess1, TMessage1>(
            Func<TSuccess, Func<TMessage, TSuccess1>> func,
            Result<TSuccess, TMessage1> first,
            Result<TMessage, TMessage1> second)
        {
            return Apply(Lift(func, first), second);
        }

        public static Result<IEnumerable<TSuccess>, TMessage> Collect<TSuccess, TMessage>(
            IEnumerable<Result<TSuccess, TMessage>> xs)
        {
            return Lift(Enumerable.Reverse,
                xs.Aggregate<Result<TSuccess, TMessage>, Result<IEnumerable<TSuccess>, TMessage>, Result<IEnumerable<TSuccess>, TMessage>>(
                null,
                (result, next) =>
                {
                    if (result.Tag == ResultType.Ok && next.Tag == ResultType.Ok)
                    {
                        var ok1 = (Ok<IEnumerable<TSuccess>, TMessage>)result;
                        var ok2 = (Ok<TSuccess, TMessage>)next;
                        return
                            new Ok<IEnumerable<TSuccess>, TMessage>(
                                    Enumerable.Empty<TSuccess>().Concat(new[] { ok2.Success }).Concat(ok1.Success),
                                    ok1.Messages.Concat(ok2.Messages));
                    }
                    if ((result.Tag == ResultType.Ok && next.Tag == ResultType.Bad)
                        || (result.Tag == ResultType.Bad && next.Tag == ResultType.Ok))
                    {
                        var m1 = result.Tag == ResultType.Ok
                            ? ((Ok<IEnumerable<TSuccess>, TMessage>)result).Messages
                            : ((Bad<TSuccess, TMessage>)next).Messages;
                        var m2 = result.Tag == ResultType.Bad
                            ? ((Bad<IEnumerable<TSuccess>, TMessage>)result).Messages
                            : ((Ok<TSuccess, TMessage>)next).Messages;
                        return new Bad<IEnumerable<TSuccess>, TMessage>(m1.Concat(m2));
                    }

                    var bad1 = (Bad<IEnumerable<TSuccess>, TMessage>)result;
                    var bad2 = (Bad<TSuccess, TMessage>)next;
                    return new Bad<IEnumerable<TSuccess>, TMessage>(bad1.Messages.Concat(bad2.Messages));
                }, x => x));
        }
    }

    static class ResultExtensions
    {
        public static void Match<TSuccess, TMessage>(this Result<TSuccess, TMessage> result,
            Action<TSuccess, IEnumerable<TMessage>> ifSuccess,
            Action<IEnumerable<TMessage>> ifFailure)
        {
            if (ifSuccess == null) throw new ArgumentException(nameof(ifSuccess));
            if (ifFailure == null) throw new ArgumentException(nameof(ifFailure));

            var ok = result as Ok<TSuccess, TMessage>;
            if (ok != null)
            {
                ifSuccess(ok.Success, ok.Messages);
                return;
            }

            var bad = (Bad<TSuccess, TMessage>)result;
            ifFailure(bad.Messages);
        }

        public static TResult Either<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, IEnumerable<TMessage>, TResult> ifSuccess,
            Func<IEnumerable<TMessage>, TResult> ifFailure)
        {
            return Trial.Either(ifSuccess, ifFailure, result);
        }

        public static Result<TResult, TMessage> Map<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, TResult> func)
        {
            return Trial.Lift(func, result);
        }


        public static Result<IEnumerable<TSuccess>, TMessage> Collect<TSuccess, TMessage>(
            this IEnumerable<Result<TSuccess, TMessage>> values)
        {
            return Trial.Collect(values);
        }

        public static Result<IEnumerable<TSuccess>, TMessage> Flatten<TSuccess, TMessage>(this Result<IEnumerable<Result<TSuccess, TMessage>>, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<IEnumerable<Result<TSuccess, TMessage>>, TMessage>)result;
                var values = ok.Success;
                var result1 = Collect(values);
                if (result1.Tag == ResultType.Ok)
                {
                    var ok1 = (Ok<IEnumerable<TSuccess>, TMessage>)result1;
                    return new Ok<IEnumerable<TSuccess>, TMessage>(ok1.Success, ok1.Messages);
                }
                var bad1 = (Bad<IEnumerable<TSuccess>, TMessage>)result1;
                return new Bad<IEnumerable<TSuccess>, TMessage>(bad1.Messages);
            }
            var bad = (Bad<IEnumerable<Result<TSuccess, TMessage>>, TMessage>)result;
            return new Bad<IEnumerable<TSuccess>, TMessage>(bad.Messages);
        }

        public static Result<TResult, TMessage> SelectMany<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, Result<TResult, TMessage>> func)
        {
            return Trial.Bind(func, result);
        }

        public static Result<TResult, TMessage> SelectMany<TSuccess, TMessage, TValue, TResult>(
            this Result<TSuccess, TMessage> result,
            Func<TSuccess, Result<TValue, TMessage>> func,
            Func<TSuccess, TValue, TResult> mapperFunc)
        {
            if (func == null) throw new ArgumentException(nameof(func));
            if (mapperFunc == null) throw new ArgumentException(nameof(mapperFunc));

            Func<TSuccess, Func<TValue, TResult>> curriedMapper = suc => val => mapperFunc(suc, val);
            Func<
                Result<TSuccess, TMessage>,
                Result<TValue, TMessage>,
                Result<TResult, TMessage>
            > liftedMapper = (a, b) => Trial.Lift2(curriedMapper, a, b);
            var v = Trial.Bind(func, result);
            return liftedMapper(result, v);
        }

        public static Result<TResult, TMessage> Select<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, TResult> func)
        {
            return Trial.Lift(func, result);
        }

        public static IEnumerable<TMessage> FailedWith<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<TSuccess, TMessage>)result;
                throw new Exception(
                    string.Format("Result was a success: {0} - {1}",
                    ok.Success,
                    string.Join(Environment.NewLine, ok.Messages.Select(m => m.ToString()))));
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            return bad.Messages;
        }

        public static TSuccess SucceededWith<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<TSuccess, TMessage>)result;
                return ok.Success;
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            throw new Exception(
                string.Format("Result was an error: {0}",
                string.Join(Environment.NewLine, bad.Messages.Select(m => m.ToString()))));
        }

        public static IEnumerable<TMessage> SuccessMessages<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<TSuccess, TMessage>)result;
                return ok.Messages;
            }
            return Enumerable.Empty<TMessage>();
        }

        public static Maybe<TSuccess> ToMaybe<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<TSuccess, TMessage>)result;
                return Maybe.Just(ok.Success);
            }
            return Maybe.Nothing<TSuccess>();
        }
    }
}

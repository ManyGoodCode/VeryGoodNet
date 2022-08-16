using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandLine
{
    public static partial class ParserResultExtensions
    {
#if !NET40
        public static async Task<ParserResult<T>> WithParsedAsync<T>(this ParserResult<T> result, Func<T, Task> action)
        {
            if (result is Parsed<T> parsed)
            {
                await action(parsed.Value);
            }
            return result;
        }

        public static async Task<ParserResult<object>> WithParsedAsync<T>(this ParserResult<object> result, Func<T, Task> action)
        {
            if (result is Parsed<object> parsed)
            {
                if (parsed.Value is T value)
                {
                    await action(value);
                }
            }
            return result;
        }

        public static async Task<ParserResult<T>> WithNotParsedAsync<T>(this ParserResult<T> result, Func<IEnumerable<Error>, Task> action)
        {
            if (result is NotParsed<T> notParsed)
            {
                await action(notParsed.Errors);
            }
            return result;
        }
#endif
    }
}

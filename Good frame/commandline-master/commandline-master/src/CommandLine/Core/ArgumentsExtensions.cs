using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    static class ArgumentsExtensions
    {
        public static IEnumerable<Error> Preprocess(
            this IEnumerable<string> arguments,
            IEnumerable<
                Func<IEnumerable<string>, IEnumerable<Error>>
                > preprocessorLookup)
        {
            return preprocessorLookup.TryHead().MapValueOrDefault(
                func: func =>
                    {
                        IEnumerable<Error> errors = func(arguments);
                        return errors.Any()
                            ? errors
                            : arguments.Preprocess(preprocessorLookup.TailNoFail());
                    },
                noneValue: Enumerable.Empty<Error>());
        }
    }
}

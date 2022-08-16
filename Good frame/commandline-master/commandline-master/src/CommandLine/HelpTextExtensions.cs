using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine
{
    public static class HelpTextExtensions
    {
        public static bool IsHelp(this IEnumerable<Error> errs)
        {
            if (errs.Any(x => x.Tag == ErrorType.HelpRequestedError ||
                            x.Tag == ErrorType.HelpVerbRequestedError))
                return true;
            return errs.Any(x => (x is UnknownOptionError ee ? ee.Token : "") == "help");
        }

        public static bool IsVersion(this IEnumerable<Error> errs)
        {
            if (errs.Any(x => x.Tag == ErrorType.VersionRequestedError))
                return true;
            return errs.Any(x => (x is UnknownOptionError ee ? ee.Token : "") == "version");
        }

        public static TextWriter Output(this IEnumerable<Error> errs)
        {
            if (errs.IsHelp() || errs.IsVersion())
                return Console.Out;
            return Console.Error;
        }
    }
}



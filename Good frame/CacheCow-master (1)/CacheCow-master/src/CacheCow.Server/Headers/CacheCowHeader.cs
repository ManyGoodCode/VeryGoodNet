using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CacheCow.Server.Headers
{
    public class CacheCowHeader
    {
        public const string Name = "x-cachecow-server";
        private const string Pattern = "validation-applied=(True|False);validation-matched=(True|False);short-circuited=(True|False);query-made=(True|False)";
        private static Regex regex = new Regex(Pattern);

        public bool ShortCircuited { get; set; }
        public bool ValidationApplied { get; set; }
        public bool ValidationMatched { get; set; }
        public bool QueryMadeAndSuccessful { get; set; }

        public override string ToString()
        {
            return $"validation-applied={ValidationApplied};validation-matched={ValidationMatched};short-circuited={ShortCircuited};query-made={QueryMadeAndSuccessful}";
        }

        public static bool TryParse(string value, out CacheCowHeader header)
        {
            header = null;
            Match m = regex.Match(value);
            if(m.Success)
            {
                header = new CacheCowHeader()
                {
                    ShortCircuited = bool.Parse(m.Groups[3].Value),
                    ValidationApplied = bool.Parse(m.Groups[1].Value),
                    ValidationMatched = bool.Parse(m.Groups[2].Value),
                    QueryMadeAndSuccessful = bool.Parse(m.Groups[4].Value)
                };

                return true;
            }

            return false;
        }
    }
}

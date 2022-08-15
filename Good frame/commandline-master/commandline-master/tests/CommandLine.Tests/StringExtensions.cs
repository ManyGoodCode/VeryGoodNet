using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Tests
{
    static class StringExtensions
    {
        public static string[] ToNotEmptyLines(this string value)
        {
            return value.Split(new[] 
            { 
                Environment.NewLine 
            },
            StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] ToLines(this string value)
        {
            return value.Split(new[] 
            { 
                Environment.NewLine
            }, 

            StringSplitOptions.None);
        }

        public static string[] TrimStringArray(this IEnumerable<string> array)
        {
            return array.Select(item => item.Trim()).ToArray();
        }
    }
}

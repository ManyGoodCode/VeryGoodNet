namespace OxyPlot
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class StringHelper
    {
        private static readonly Regex FormattingExpression = new Regex("{(?<Property>.+?)(?<Format>\\:.*?)?}");

        public static string Format(IFormatProvider provider, string formatString, object item, params object[] values)
        {
            var s = FormattingExpression.Replace(
                formatString,
                delegate (Match match)
                {
                    var property = match.Groups["Property"].Value;
                    if (property.Length > 0 && char.IsDigit(property[0]))
                    {
                        return match.Value;
                    }

                    var pi = item.GetType().GetRuntimeProperty(property);
                    if (pi == null)
                    {
                        return string.Empty;
                    }

                    var v = pi.GetValue(item, null);
                    var format = match.Groups["Format"].Value;

                    var fs = "{0" + format + "}";
                    return string.Format(provider, fs, v);
                });

            s = string.Format(provider, s, values);
            return s;
        }

        public static string CreateValidFormatString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "{0}";
            }

            if (input.Contains("{"))
            {
                return input;
            }

            return string.Concat("{0:", input, "}");
        }

        public static IEnumerable<string> Format(this IEnumerable source, string propertyName, string formatString, IFormatProvider provider)
        {
            var fs = CreateValidFormatString(formatString);
            if (string.IsNullOrEmpty(propertyName))
            {
                foreach (var element in source)
                {
                    yield return string.Format(provider, fs, element);
                }
            }
            else
            {
                var reflectionPath = new ReflectionPath(propertyName);
                foreach (var element in source)
                {
                    var value = reflectionPath.GetValue(element);
                    yield return string.Format(provider, fs, value);
                }
            }
        }

        public static string[] SplitLines(string text)
        {
            return Regex.Split(text, "\r?\n");
        }
    }
}

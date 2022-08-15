using System;
using System.Collections.Generic;
using System.Linq;

namespace Fclp.Internals.Extensions
{
	public static class UsefulExtension
	{
		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim());
		}

		public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> enumerable)
		{
			return enumerable == null || enumerable.Any() == false;
		}

		public static void ForEach<TSource>(this IEnumerable<TSource> enumerable, Action<TSource> action)
		{
			foreach (TSource item in enumerable)
			{
				action(item);
			}
		}

		public static bool ContainsWhitespace(this string value)
		{
			return string.IsNullOrEmpty(value) == false && value.Contains(" ");
		}

		public static string WrapInDoubleQuotes(this string str)
		{
			return string.Format(@"""{0}""", str);
		}

		public static string RemoveAnyWrappingDoubleQuotes(this string str)
		{

			if (!str.IsNullOrWhiteSpace())
			{
				if (str.StartsWith("\"") && str.EndsWith("\""))
					return str.Substring(1, str.Length - 2);
			}

			return str;
		}

		public static string WrapInDoubleQuotesIfContainsWhitespace(this string str)
		{
			return str.ContainsWhitespace() && str.IsWrappedInDoubleQuotes() == false
				? str.WrapInDoubleQuotes()
				: str;
		}

		public static bool IsWrappedInDoubleQuotes(this string str)
		{
			return str.IsNullOrWhiteSpace() == false && str.StartsWith("\"") && str.EndsWith("\"");
		}

		public static IEnumerable<string> SplitOnWhitespace(this string value)
		{
			if (string.IsNullOrEmpty(value)) 
				return null;
			char[] parmChars = value.ToCharArray();
			bool inDoubleQuotes = false;
			for (int index = 0; index < parmChars.Length; index++)
			{
				if (parmChars[index] == '"')
					inDoubleQuotes = !inDoubleQuotes;
				if (!inDoubleQuotes && parmChars[index] == ' ')
					parmChars[index] = '\n';
			}

			return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static T ElementAtOrDefault<T>(this T[] items, int index, T defaultToUse)
		{
		    if (items == null) 
				return defaultToUse;
			return index >= 0 && index < items.Length
				? items[index]
				: defaultToUse;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing.OptionParsers
{
    /// <summary>
    /// 枚举标志解析器
    /// </summary>
    public class EnumFlagCommandLineOptionParser<TEnum> : ICommandLineOptionParser<TEnum>
    {
        private readonly IList<TEnum> all;
        private readonly Dictionary<string, TEnum> insensitiveNames;
        private readonly Dictionary<int, TEnum> values;

        public EnumFlagCommandLineOptionParser()
        {
            Type type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException(string.Format("T must be an System.Enum but is '{0}'", type));

            if (!type.IsDefined(typeof(FlagsAttribute), false))
                throw new ArgumentException("T must have a System.FlagsAttribute'");

            all = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            insensitiveNames = all.ToDictionary(k => Enum.GetName(typeof(TEnum), k).ToLowerInvariant());
            values = all.ToDictionary(k => Convert.ToInt32(k));
        }

        public TEnum Parse(ParsedOption parsedOption)
        {
            int result = 0;
            foreach (string value in parsedOption.Values)
            {
                result += (int)Enum.Parse(typeof(TEnum), value.ToLowerInvariant(), true);
            }

            return (TEnum)(object)result;

        }

        public bool CanParse(ParsedOption parsedOption)
        {
            if (parsedOption == null)
                return false;
            if (parsedOption.HasValue == false)
                return false;

            return parsedOption.Values.All(
            value =>
            {
                if (value.IsNullOrWhiteSpace())
                    return false;
                return IsDefined(value);
            });
        }

        private bool IsDefined(string value)
        {
            int asInt;
            return int.TryParse(value, out asInt)
                ? IsDefined(asInt)
                : insensitiveNames.Keys.Contains(value.ToLowerInvariant());
        }

        private bool IsDefined(int value)
        {
            return values.Keys.Contains(value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;

namespace Fclp.Internals.Parsing.OptionParsers
{
    /// <summary>
    /// 枚举解析器通过 Enum.TryParse 和 判断字符串名称 是否包含在枚举中确定
    /// </summary>
    public class EnumCommandLineOptionParser<TEnum> : ICommandLineOptionParser<TEnum>
    {
        private readonly IList<TEnum> allEnumValues;
        private readonly Dictionary<string, TEnum> insensitiveNames;
        private readonly Dictionary<int, TEnum> values;

        public EnumCommandLineOptionParser()
        {
            Type type = typeof(TEnum);
            // 判断Type类型是否为枚举
            if (!type.IsEnum)
                throw new ArgumentException(string.Format("T must be an System.Enum but is '{0}'", type));
            allEnumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            insensitiveNames = allEnumValues
                .ToDictionary(k => Enum.GetName(enumType: typeof(TEnum), value: k)
                .ToLowerInvariant());

            // 将集合字节转换为字典 keySelector 委托
            values = allEnumValues.ToDictionary(keySelector: k => Convert.ToInt32(k));
        }

        public TEnum Parse(ParsedOption parsedOption)
        {
            return (TEnum)Enum.Parse(
                enumType: typeof(TEnum),
                value: parsedOption.Value.ToLowerInvariant(),
                //忽略大小写
                ignoreCase: true);
        }

        public bool CanParse(ParsedOption parsedOption)
        {
            if (parsedOption.HasValue == false)
                return false;
            if (parsedOption.Value.IsNullOrWhiteSpace())
                return false;
            return IsDefined(parsedOption.Value);
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
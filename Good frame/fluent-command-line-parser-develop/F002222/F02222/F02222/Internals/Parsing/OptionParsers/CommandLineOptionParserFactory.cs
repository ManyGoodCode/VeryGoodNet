using System;
using System.Collections.Generic;

namespace Fclp.Internals.Parsing.OptionParsers
{
    public class CommandLineOptionParserFactory : ICommandLineOptionParserFactory
    {
        internal Dictionary<Type, object> Parsers { get; set; }

        public CommandLineOptionParserFactory()
        {
            this.Parsers = new Dictionary<Type, object>();
            this.AddOrReplace(new BoolCommandLineOptionParser());
            this.AddOrReplace(new Int32CommandLineOptionParser());
            this.AddOrReplace(new Int64CommandLineOptionParser());
            this.AddOrReplace(new StringCommandLineOptionParser());
            this.AddOrReplace(new DateTimeCommandLineOptionParser());
            this.AddOrReplace(new TimeSpanCommandLineOptionParser());
            this.AddOrReplace(new DoubleCommandLineOptionParser());
            this.AddOrReplace(new UriCommandLineOptionParser());

            this.AddOrReplace(new ListCommandLineOptionParser<string>(this));
            this.AddOrReplace(new ListCommandLineOptionParser<int>(this));
            this.AddOrReplace(new ListCommandLineOptionParser<long>(this));
            this.AddOrReplace(new ListCommandLineOptionParser<double>(this));
            this.AddOrReplace(new ListCommandLineOptionParser<DateTime>(this));
            this.AddOrReplace(new ListCommandLineOptionParser<TimeSpan>(this));
            this.AddOrReplace(new ListCommandLineOptionParser<bool>(this));
            this.AddOrReplace(new ListCommandLineOptionParser<Uri>(this));


            this.AddOrReplace(new NullableCommandLineOptionParser<bool>(this));
            this.AddOrReplace(new NullableCommandLineOptionParser<int>(this));
            this.AddOrReplace(new NullableCommandLineOptionParser<long>(this));
            this.AddOrReplace(new NullableCommandLineOptionParser<double>(this));
            this.AddOrReplace(new NullableCommandLineOptionParser<DateTime>(this));
            this.AddOrReplace(new NullableCommandLineOptionParser<TimeSpan>(this));
        }

        public void AddOrReplace<T>(ICommandLineOptionParser<T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            Type parserType = typeof(T);
            this.Parsers.Remove(parserType);
            this.Parsers.Add(parserType, parser);
        }

        /// <summary>
        /// 返回指定的解析器
        /// </summary>
        public ICommandLineOptionParser<T> CreateParser<T>()
        {
            Type type = typeof(T);
            if (!this.Parsers.ContainsKey(type))
            {
                if (!TryAddAsSpecialParser<T>(type))
                {
                    throw new UnsupportedTypeException();
                }
            }

            return (ICommandLineOptionParser<T>)this.Parsers[type];
        }

        private bool TryAddAsSpecialParser<T>(Type type)
        {
            // 判断是否是枚举
            if (type.IsEnum)
            {
                bool hasFlags = typeof(T).IsDefined(typeof(FlagsAttribute), false);
                Type enumParserType = hasFlags ?
                    typeof(EnumFlagCommandLineOptionParser<T>) :
                    typeof(EnumCommandLineOptionParser<T>);

                if (!Parsers.ContainsKey(enumParserType))
                {
                    if (hasFlags)
                    {
                        this.AddOrReplace(new EnumFlagCommandLineOptionParser<T>());
                    }
                    else
                    {
                        this.AddOrReplace(new EnumCommandLineOptionParser<T>());
                    }

                }

                return true;
            }

            // 判断是否为可空类型
            if (IsNullableEnum(type))
            {
                Type underlyingType = Nullable.GetUnderlyingType(type);
                Type nullableEnumParserType = typeof(NullableEnumCommandLineOptionParser<>).MakeGenericType(underlyingType);
                ICommandLineOptionParser<T> parser = (ICommandLineOptionParser<T>)Activator.CreateInstance(nullableEnumParserType, this);
                if (!this.Parsers.ContainsKey(type))
                {
                    this.AddOrReplace(parser);
                }

                return true;
            }

            if (type.IsGenericType)
            {
                Type genericType = TryGetListGenericType(type);
                if (genericType != null)
                {
                    if (genericType.IsEnum || IsNullableEnum(genericType))
                    {
                        Type enumListParserType = typeof(ListCommandLineOptionParser<>).MakeGenericType(genericType);
                        ICommandLineOptionParser<T> parser = (ICommandLineOptionParser<T>)Activator.CreateInstance(enumListParserType, this);
                        if (!this.Parsers.ContainsKey(type))
                        {
                            this.AddOrReplace(parser);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsNullableEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }

        private static Type TryGetListGenericType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition()
                == typeof(List<>))
            {
                return type.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
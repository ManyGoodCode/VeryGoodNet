using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine.Core;
using CSharpx;

namespace CommandLine.Infrastructure
{
    static class ReflectionHelper
    {
        [ThreadStatic] private static IDictionary<Type, Attribute> _overrides;
        public static void SetAttributeOverride(IEnumerable<Attribute> overrides)
        {
            if (overrides != null)
            {
                _overrides = overrides.ToDictionary(attr => attr.GetType(), attr => attr);
            }
            else
            {
                _overrides = null;
            }
        }

        public static Maybe<TAttribute> GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            // Test support
            if (_overrides != null)
            {
                return
                    _overrides.ContainsKey(typeof(TAttribute)) ?
                        Maybe.Just((TAttribute)_overrides[typeof(TAttribute)]) :
                        Maybe.Nothing<TAttribute>();
            }

            var assembly = GetExecutingOrEntryAssembly();

#if NET40
            var attributes = assembly.GetCustomAttributes(typeof(TAttribute), false);
#else
            var attributes = assembly.GetCustomAttributes<TAttribute>().ToArray();
#endif

            return attributes.Length > 0
                ? Maybe.Just((TAttribute)attributes[0])
                : Maybe.Nothing<TAttribute>();
        }

        public static string GetAssemblyName()
        {
            var assembly = GetExecutingOrEntryAssembly();
            return assembly.GetName().Name;
        }

        public static string GetAssemblyVersion()
        {
            var assembly = GetExecutingOrEntryAssembly();
            return assembly.GetName().Version.ToStringInvariant();
        }

        public static bool IsFSharpOptionType(Type type)
        {
            return type.FullName.StartsWith(
                "Microsoft.FSharp.Core.FSharpOption`1", StringComparison.Ordinal);
        }

        public static T CreateDefaultImmutableInstance<T>(Type[] constructorTypes)
        {
            var t = typeof(T);
            return (T)CreateDefaultImmutableInstance(t, constructorTypes);
        }

        public static object CreateDefaultImmutableInstance(Type type, Type[] constructorTypes)
        {
            var ctor = type.GetTypeInfo().GetConstructor(constructorTypes);
            if (ctor == null)
            {
                throw new InvalidOperationException($"Type {type.FullName} appears to be immutable, but no constructor found to accept values.");
            }

            var values = (from prms in ctor.GetParameters()
                          select prms.ParameterType.CreateDefaultForImmutable()).ToArray();
            return ctor.Invoke(values);
        }

        private static Assembly GetExecutingOrEntryAssembly()
        {
            return Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        }

       public static IEnumerable<string> GetNamesOfEnum(Type t)
        {
            if (t.IsEnum)
                return Enum.GetNames(t);
            Type u = Nullable.GetUnderlyingType(t);
            if (u != null && u.IsEnum)
                return Enum.GetNames(u);
            return Enumerable.Empty<string>();
        }
    }
}

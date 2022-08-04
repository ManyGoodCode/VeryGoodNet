using F002438.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace F002438.Extensions
{
    public static class TypeExtensions
    {
        private static SafeDictionary<GenericMethodCacheKey, MethodInfo> genericMethodCache;

        static TypeExtensions()
        {
            genericMethodCache = new SafeDictionary<GenericMethodCacheKey, MethodInfo>();
        }

        public static MethodInfo GetGenericMethod(
            this Type sourceType,
            BindingFlags bindingFlags,
            string methodName,
            Type[] genericTypes,
            Type[] parameterTypes)
        {
            MethodInfo method;
            GenericMethodCacheKey cacheKey = new GenericMethodCacheKey(sourceType, methodName, genericTypes, parameterTypes);

            if (!genericMethodCache.TryGetValue(cacheKey, out method))
            {
                method = GetMethod(sourceType, bindingFlags, methodName, genericTypes, parameterTypes);
                genericMethodCache[cacheKey] = method;
            }

            return method;
        }

        private static MethodInfo GetMethod(Type sourceType, BindingFlags bindingFlags, string methodName, Type[] genericTypes, Type[] parameterTypes)
        {
            IEnumerable<MethodInfo> validMethods = from method in sourceType.GetMethods(bindingFlags)
                               where method.Name == methodName
                               where method.IsGenericMethod
                               where method.GetGenericArguments().Length == genericTypes.Length
                               let genericMethod = method.MakeGenericMethod(genericTypes)
                               let genericMethodParameters = genericMethod.GetParameters()
                               where genericMethodParameters.Length == parameterTypes.Length
                               where genericMethodParameters.Select(pi => pi.ParameterType).SequenceEqual(parameterTypes)
                               select genericMethod;

            List<MethodInfo> methods = validMethods.ToList();
            if (methods.Count > 1)
            {
                throw new AmbiguousMatchException();
            }

            return methods.FirstOrDefault();
        }

        private sealed class GenericMethodCacheKey
        {
            private readonly Type sourceType;
            private readonly string methodName;
            private readonly Type[] genericTypes;
            private readonly Type[] parameterTypes;
            private readonly int hashCode;

            public GenericMethodCacheKey(Type sourceType, string methodName, Type[] genericTypes, Type[] parameterTypes)
            {
                this.sourceType = sourceType;
                this.methodName = methodName;
                this.genericTypes = genericTypes;
                this.parameterTypes = parameterTypes;
                hashCode = GenerateHashCode();
            }

            public override bool Equals(object obj)
            {
                GenericMethodCacheKey cacheKey = obj as GenericMethodCacheKey;
                if (cacheKey == null)
                    return false;

                if (sourceType != cacheKey.sourceType)
                    return false;

                if (!string.Equals(methodName, cacheKey.methodName, StringComparison.Ordinal))
                    return false;

                if (genericTypes.Length != cacheKey.genericTypes.Length)
                    return false;

                if (parameterTypes.Length != cacheKey.parameterTypes.Length)
                    return false;

                for (int i = 0; i < genericTypes.Length; ++i)
                {
                    if (genericTypes[i] != cacheKey.genericTypes[i])
                        return false;
                }

                for (int i = 0; i < parameterTypes.Length; ++i)
                {
                    if (parameterTypes[i] != cacheKey.parameterTypes[i])
                        return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                return hashCode;
            }

            private int GenerateHashCode()
            {
                unchecked
                {
                    int result = sourceType.GetHashCode();
                    result = (result * 397) ^ methodName.GetHashCode();
                    for (int i = 0; i < genericTypes.Length; ++i)
                    {
                        result = (result * 397) ^ genericTypes[i].GetHashCode();
                    }

                    for (int i = 0; i < parameterTypes.Length; ++i)
                    {
                        result = (result * 397) ^ parameterTypes[i].GetHashCode();
                    }

                    return result;
                }
            }
        }
    }
}

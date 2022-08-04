using F002438.CoreException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace F002438.Entity
{
    public static class TinyIoCReflectionCache
    {
        private static readonly SafeDictionary<Type, ConstructorInfo[]> _UsableConstructors = new SafeDictionary<Type, ConstructorInfo[]>();
        private static readonly SafeDictionary<string, Type> _GenericTypes = new SafeDictionary<string, Type>();

        public static IEnumerable<ConstructorInfo> GetUsableConstructors(Type type)
        {
            if (!_UsableConstructors.TryGetValue(type, out ConstructorInfo[] constructors))
            {
                List<ConstructorInfo> candidateCtors = type.GetConstructors(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    )
                    .Where(x => !x.IsPrivate)  // Includes internal constructors but not private constructors
                    .Where(x => !x.IsFamily)   // Excludes【排除】 protected constructors
                    .ToList();

                List<ConstructorInfo> attributeCtors = candidateCtors.Where(x => x.GetCustomAttributes(typeof(TinyIoCConstructorAttribute), false).Any())
                    .ToList();

                if (attributeCtors.Any())
                    candidateCtors = attributeCtors;

                constructors = candidateCtors.OrderByDescending(ctor => ctor.GetParameters().Length).ToArray();
                _UsableConstructors[type] = constructors;
            }

            return constructors;
        }

        internal static Type GetGenericImplementationType(Type typeToConstruct, Type requestedType)
        {
            string key = typeToConstruct.FullName + ":" + requestedType.FullName;
            if (!_GenericTypes.TryGetValue(key, out Type retVal))
            {
                Type[] genericTypeArguments = null;
                if (requestedType == null
                    || !requestedType.IsGenericType()
                    || !(genericTypeArguments = requestedType.GetGenericArguments()).Any())
                    throw new TinyIoCResolutionException(typeToConstruct);

                retVal = typeToConstruct.MakeGenericType(genericTypeArguments);
                _GenericTypes[key] = retVal;
            }

            return retVal;
        }
    }
}

using System.Collections.Generic;

#if BEFORE_NET45
namespace System.Reflection
{
    internal static class SharpDXTypeExtensions
    {
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            var attrs = type.GetCustomAttributes(typeof(T), true);
            if(attrs.Length == 0)
            {
                return null;
            }
            return (T)attrs[0];
        }

        public static T GetCustomAttribute<T>(this MemberInfo memberInfo, bool inherited) where T : Attribute
        {
            var attrs = memberInfo.GetCustomAttributes(typeof(T), inherited);
            if (attrs.Length == 0)
            {
                return null;
            }
            return (T)attrs[0];
        }
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo memberInfo, bool inherited) where T : Attribute
        {
            var attrs = memberInfo.GetCustomAttributes(typeof(T), inherited);
            foreach(var attr in attrs)
            {
                yield return (T)attr;
            }
        }
    }
}
#endif
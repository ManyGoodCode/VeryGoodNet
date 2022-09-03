using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace WebApiContrib.Testing.Internal.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static string ActionName(this MethodInfo method)
        {
            if (method.HasAttribute<ActionNameAttribute>())
                return method.GetAttribute<ActionNameAttribute>().First().Name;

            return method.Name;
        }
    }
}
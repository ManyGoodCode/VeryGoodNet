using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExpressionStep5
{
    public class TypeHelp
    {
        public static Type FindGenericType(Type type)
        {
            if (type == null || type == typeof(string) || type.IsArray)
                return null;
            if (type.IsGenericType)
            {
                Type[] types = type.GetGenericArguments();
                foreach (Type t in types)
                {
                    return t;
                }
            }

            return null;
        }
    }
}

namespace OxyPlot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Type的扩展方法
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 查找Type的指定名称属性
        /// </summary>
        public static PropertyInfo GetRuntimeProperty(this Type type, string name)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            IEnumerable<PropertyInfo> sources = typeInfo.AsType().GetRuntimeProperties();

            foreach (PropertyInfo x in sources)
            {
                if (x.Name == name) 
                    return x;
            }

            return null;
        }
    }
}

using System;
using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Extensions
{
    /// <summary>
    /// 获取枚举属性上的描述符特性说明值
    /// </summary>
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this Enum val)
        {
            DescriptionAttribute[] attributes = new DescriptionAttribute[0];
            System.Reflection.FieldInfo? fieldInfo = val.GetType().GetField(val.ToString());
            if (fieldInfo == null || fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0)
            {
                return val.ToString();
            }
            else
            {
                attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attributes[0].Description;
            }
        }
    }
}

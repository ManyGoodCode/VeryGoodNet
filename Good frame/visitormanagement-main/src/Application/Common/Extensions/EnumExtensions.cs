// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Extensions
{

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

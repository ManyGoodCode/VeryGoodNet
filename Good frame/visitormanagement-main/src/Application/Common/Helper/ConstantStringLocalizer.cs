using System.Globalization;
using System.Resources;

namespace CleanArchitecture.Blazor.Application.Common.Helper
{
    /// <summary>
    /// 根据静态构造器初始化  静态，只读的本地对象
    /// 通过调用静态 Localize(string key) 函数实现本地化
    /// </summary>
    public static class ConstantStringLocalizer
    {
        public const string CONSTANTSTRINGRESOURCEID = "CleanArchitecture.Blazor.Application.Resources.Constants.ConstantString";
        private static readonly ResourceManager rm;

        static ConstantStringLocalizer()
        {
            rm = new ResourceManager(baseName: CONSTANTSTRINGRESOURCEID, assembly: typeof(ConstantStringLocalizer).Assembly);
        }
        public static string Localize(string key)
        {
            return rm.GetString(key, CultureInfo.CurrentCulture) ?? key;
        }
    }
}

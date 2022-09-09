using System;

namespace CleanArchitecture.Blazor.Application.Common.Security
{
    /// <summary>
    /// 请求权限特性。 角色集 / 策略
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequestAuthorizeAttribute : Attribute
    {
        public RequestAuthorizeAttribute() { }

        public string Roles { get; set; }

        public string Policy { get; set; }
    }
}
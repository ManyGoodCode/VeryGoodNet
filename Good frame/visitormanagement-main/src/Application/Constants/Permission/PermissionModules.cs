
using System.Collections.Generic;

namespace CleanArchitecture.Blazor.Application.Constants.Permission
{
    /// <summary>
    /// 1. 根据模块名称获取权限。Permissions.{module}.Create / Permissions.{module}.View / Permissions.{module}.Edit / Permissions.{module}.Delete
    /// 2. 获取模块的所有权限
    /// 3. 常量字符串
    /// </summary>
    public static class PermissionModules
    {
        /// <summary>
        /// 根据模块名称获取权限。Permissions.{module}.Create / Permissions.{module}.View / Permissions.{module}.Edit / Permissions.{module}.Delete
        /// </summary>
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete"
            };
        }

        /// <summary>
        /// 获取模块的所有权限
        /// </summary>
        public static List<string> GetAllPermissionsModules()
        {
            return new List<string>()
            {
                Users,
                Roles,
                Products,
                Brands,
                Companies
            };
        }

        public const string Users = "Users";
        public const string Roles = "Roles";
        public const string Products = "Products";
        public const string Brands = "Brands";
        public const string Companies = "Companies";
    }
}

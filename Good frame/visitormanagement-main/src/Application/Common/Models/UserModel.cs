using System;

namespace CleanArchitecture.Blazor.Application.Common.Models
{
    /// <summary>
    /// 用户名 /  用户Id / 显示名称 / 邮件地址 / 电话号码 / 角色 / 是否在线 / 部门 / 最近登录时间
    /// </summary>

    public class UserModel
    {
        public string? Site { get; set; }
        public string? Avatar { get; set; }
        public string? DisplayName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string[]? Roles { get; set; }
        public string? UserId { get; set; }
        public int? SiteId { get; set; }
        public bool IsActive { get; set; }
        public bool IsLive { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
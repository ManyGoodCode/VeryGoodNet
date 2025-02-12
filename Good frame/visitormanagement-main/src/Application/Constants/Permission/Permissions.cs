using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CleanArchitecture.Blazor.Application.Constants.Permission
{
    /// <summary>
    /// 获取所有权限的字符串组合
    /// </summary>
    public static class Permissions
    {
        [DisplayName(displayName: "AuditTrails")]
        [Description(description: "AuditTrails Permissions")]
        public static class AuditTrails
        {
            public const string View = "Permissions.AuditTrails.View";
            public const string Search = "Permissions.AuditTrails.Search";
            public const string Export = "Permissions.AuditTrails.Export";
        }

        [DisplayName(displayName: "Logs")]
        [Description(description: "Logs Permissions")]
        public static class Logs
        {
            public const string View = "Permissions.Logs.View";
            public const string Search = "Permissions.Logs.Search";
            public const string Export = "Permissions.Logs.Export";
        }

        [DisplayName(displayName: "VisitorHistories")]
        [Description(description: "VisitorHistories Permissions")]
        public static class VisitorHistories
        {
            public const string View = "Permissions.VisitorHistories.View";
            public const string Delete = "Permissions.VisitorHistories.Delete";
            public const string Search = "Permissions.VisitorHistories.Search";
        }

        [DisplayName(displayName: "Visitors")]
        [Description(description: "Visitors Permissions")]
        public static class Visitors
        {
            public const string Detail = "Permissions.Visitors.Detail";
            public const string VisitorRequest = "Permissions.Visitors.VisitorRequest";
            public const string CompleteVisitorInfo = "Permissions.Visitors.CompleteVisitorInfo";
            public const string MyCodeView = "Permissions.Visitors.MyCodeView";
            public const string View = "Permissions.Visitors.View";
            public const string Create = "Permissions.Visitors.Create";
            public const string Edit = "Permissions.Visitors.Edit";
            public const string Delete = "Permissions.Visitors.Delete";
            public const string Search = "Permissions.Visitors.Search";
            public const string Approve = "Permissions.Visitors.Approve";
            public const string QuickApprove = "Permissions.Visitors.QuickApprove";
            public const string PendingApproval = "Permissions.Visitors.PendingApproval";
            public const string PendingCheckin = "Permissions.Visitors.PendingCheckin";
            public const string Checkin = "Permissions.Visitors.Checkin";
            public const string Checkout = "Permissions.Visitors.Checkout";
            public const string PreRegisters = "Permissions.Visitors.PreRegisters";
            public const string PendingConfirm = "Permissions.Visitors.PendingConfirm";
        }

        [DisplayName(displayName: "Devices")]
        [Description(description: "Devices Permissions")]
        public static class Devices
        {
            public const string View = "Permissions.Devices.View";
            public const string Create = "Permissions.Devices.Create";
            public const string Edit = "Permissions.Devices.Edit";
            public const string Delete = "Permissions.Devices.Delete";
            public const string Search = "Permissions.Devices.Search";

        }

        [DisplayName(displayName: "CheckinPoints")]
        [Description(description: "CheckinPoints Permissions")]
        public static class CheckinPoints
        {
            public const string View = "Permissions.CheckinPoints.View";
            public const string Create = "Permissions.CheckinPoints.Create";
            public const string Edit = "Permissions.CheckinPoints.Edit";
            public const string Delete = "Permissions.CheckinPoints.Delete";
            public const string Search = "Permissions.CheckinPoints.Search";

        }

        [DisplayName(displayName: "MessageTemplates")]
        [Description(description: "MessageTemplates Permissions")]
        public static class MessageTemplates
        {
            public const string View = "Permissions.MessageTemplates.View";
            public const string Create = "Permissions.MessageTemplates.Create";
            public const string Edit = "Permissions.MessageTemplates.Edit";
            public const string Delete = "Permissions.MessageTemplates.Delete";
            public const string Search = "Permissions.MessageTemplates.Search";
        }

        [DisplayName(displayName: "SiteConfigurations")]
        [Description(description: "SiteConfigurations Permissions")]
        public static class SiteConfigurations
        {
            public const string View = "Permissions.SiteConfigurations.View";
            public const string Create = "Permissions.SiteConfigurations.Create";
            public const string Edit = "Permissions.SiteConfigurations.Edit";
            public const string Delete = "Permissions.SiteConfigurations.Delete";
            public const string Search = "Permissions.SiteConfigurations.Search";
        }

        [DisplayName(displayName: "Sites")]
        [Description(description: "Sites Permissions")]
        public static class Sites
        {
            public const string View = "Permissions.Sites.View";
            public const string Create = "Permissions.Sites.Create";
            public const string Edit = "Permissions.Sites.Edit";
            public const string Delete = "Permissions.Sites.Delete";
            public const string Search = "Permissions.Sites.Search";

        }

        [DisplayName(displayName: "Employees")]
        [Description(description: "Employees Permissions")]
        public static class Employees
        {
            public const string View = "Permissions.Employees.View";
            public const string Create = "Permissions.Employees.Create";
            public const string Edit = "Permissions.Employees.Edit";
            public const string Delete = "Permissions.Employees.Delete";
            public const string Search = "Permissions.Employees.Search";

        }

        [DisplayName(displayName: "Designations")]
        [Description(description: "Designations Permissions")]
        public static class Designations
        {
            public const string View = "Permissions.Designations.View";
            public const string Create = "Permissions.Designations.Create";
            public const string Edit = "Permissions.Designations.Edit";
            public const string Delete = "Permissions.Designations.Delete";
            public const string Search = "Permissions.Designations.Search";
        }

        [DisplayName(displayName: "Departments")]
        [Description(description: "Departments Permissions")]
        public static class Departments
        {
            public const string View = "Permissions.Departments.View";
            public const string Create = "Permissions.Departments.Create";
            public const string Edit = "Permissions.Departments.Edit";
            public const string Delete = "Permissions.Departments.Delete";
            public const string Search = "Permissions.Departments.Search";

        }

        [DisplayName(displayName: "Products")]
        [Description(description: "Products Permissions")]
        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Create = "Permissions.Products.Create";
            public const string Edit = "Permissions.Products.Edit";
            public const string Delete = "Permissions.Products.Delete";
            public const string Search = "Permissions.Products.Search";
            public const string Export = "Permissions.Products.Export";
            public const string Import = "Permissions.Products.Import";
        }

        [DisplayName(displayName: "Customers")]
        [Description(description: "Customers Permissions")]
        public static class Customers
        {
            public const string View = "Permissions.Customers.View";
            public const string Create = "Permissions.Customers.Create";
            public const string Edit = "Permissions.Customers.Edit";
            public const string Delete = "Permissions.Customers.Delete";
            public const string Search = "Permissions.Customers.Search";
            public const string Export = "Permissions.Customers.Export";
            public const string Import = "Permissions.Customers.Import";
        }

        [DisplayName(displayName: "Categories")]
        [Description(description: "Categories Permissions")]
        public static class Categories
        {
            public const string View = "Permissions.Categories.View";
            public const string Create = "Permissions.Categories.Create";
            public const string Edit = "Permissions.Categories.Edit";
            public const string Delete = "Permissions.Categories.Delete";
            public const string Search = "Permissions.Categories.Search";
            public const string Export = "Permissions.Categories.Export";
            public const string Import = "Permissions.Categories.Import";
        }

        [DisplayName(displayName: "Documents")]
        [Description(description: "Documents Permissions")]
        public static class Documents
        {
            public const string View = "Permissions.Documents.View";
            public const string Create = "Permissions.Documents.Create";
            public const string Edit = "Permissions.Documents.Edit";
            public const string Delete = "Permissions.Documents.Delete";
            public const string Search = "Permissions.Documents.Search";
            public const string Export = "Permissions.Documents.Export";
            public const string Import = "Permissions.Documents.Import";
            public const string Download = "Permissions.Documents.Download";
        }

        [DisplayName(displayName: "DocumentTypes")]
        [Description(description: "DocumentTypes Permissions")]
        public static class DocumentTypes
        {
            public const string View = "Permissions.DocumentTypes.View";
            public const string Create = "Permissions.DocumentTypes.Create";
            public const string Edit = "Permissions.DocumentTypes.Edit";
            public const string Delete = "Permissions.DocumentTypes.Delete";
            public const string Search = "Permissions.DocumentTypes.Search";
            public const string Export = "Permissions.Documents.Export";
            public const string Import = "Permissions.Categories.Import";
        }

        [DisplayName(displayName: "Dictionaries")]
        [Description(description: "Dictionaries Permissions")]
        public static class Dictionaries
        {
            public const string View = "Permissions.Dictionaries.View";
            public const string Create = "Permissions.Dictionaries.Create";
            public const string Edit = "Permissions.Dictionaries.Edit";
            public const string Delete = "Permissions.Dictionaries.Delete";
            public const string Search = "Permissions.Dictionaries.Search";
            public const string Export = "Permissions.Dictionaries.Export";
            public const string Import = "Permissions.Dictionaries.Import";
        }

        [DisplayName(displayName: "Users")]
        [Description(description: "Users Permissions")]
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
            public const string Search = "Permissions.Users.Search";
            public const string Import = "Permissions.Users.Import";
            public const string Export = "Permissions.Dictionaries.Export";
            public const string ManageRoles = "Permissions.Users.ManageRoles";
            public const string RestPassword = "Permissions.Users.RestPassword";
            public const string Active = "Permissions.Users.Active";
        }

        [DisplayName(displayName: "Roles")]
        [Description(description: "Roles Permissions")]
        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Delete = "Permissions.Roles.Delete";
            public const string Search = "Permissions.Roles.Search";
            public const string Export = "Permissions.Roles.Export";
            public const string Import = "Permissions.Roles.Import";
            public const string ManagePermissions = "Permissions.Roles.Permissions";
            public const string ManageNavigation = "Permissions.Roles.Navigation";
        }

        [DisplayName(displayName: "Role Claims")]
        [Description(description: "Role Claims Permissions")]
        public static class RoleClaims
        {
            public const string View = "Permissions.RoleClaims.View";
            public const string Create = "Permissions.RoleClaims.Create";
            public const string Edit = "Permissions.RoleClaims.Edit";
            public const string Delete = "Permissions.RoleClaims.Delete";
            public const string Search = "Permissions.RoleClaims.Search";
        }

        [DisplayName(displayName: "Dashboards")]
        [Description(description: "Dashboards Permissions")]
        public static class Dashboards
        {
            public const string View = "Permissions.Dashboards.View";
        }

        [DisplayName(displayName: "Hangfire")]
        [Description(description: "Hangfire Permissions")]
        public static class Hangfire
        {
            public const string View = "Permissions.Hangfire.View";
        }


        public static List<string> GetRegisteredPermissions()
        {
            List<string> permissions = new List<string>();
            foreach (FieldInfo prop in typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                object? propertyValue = prop.GetValue(null);
                if (propertyValue != null)
                    permissions.Add((string)propertyValue);
            }

            return permissions;
        }
    }
}

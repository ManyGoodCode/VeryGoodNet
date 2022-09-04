using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Helper;

namespace CleanArchitecture.Blazor.Application.Constants
{
    public static class ToastText
    {
        public static string SAVESUCCESS => ConstantStringLocalizer.Localize("Save successfully");
        public static string DELETESUCCESS => ConstantStringLocalizer.Localize("Delete successfully");
        public static string UPDATESUCCESS => ConstantStringLocalizer.Localize("Update successfully");
        public static string CREATESUCCESS => ConstantStringLocalizer.Localize("Create successfully");
        public static string LOGINSUCCESS => ConstantStringLocalizer.Localize("Login successfully");
        public static string LOGOUTSUCCESS => ConstantStringLocalizer.Localize("Logout successfully");
        public static string LOGINFAIL => ConstantStringLocalizer.Localize("Login fail");
        public static string LOGOUTFAIL => ConstantStringLocalizer.Localize("Logout fail");
        public static string IMPORTSUCCESS => ConstantStringLocalizer.Localize("Import successfully");
        public static string IMPORTFAIL => ConstantStringLocalizer.Localize("Import fail");
        public static string EXPORTSUCESS => ConstantStringLocalizer.Localize("Export successfully");
        public static string EXPORTFAIL => ConstantStringLocalizer.Localize("Export fail");
        public static string UPLOADSUCCESS => ConstantStringLocalizer.Localize("Upload successfully");
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Helper;

namespace CleanArchitecture.Blazor.Application.Constants
{
    /// <summary>
    /// 按钮名称本地化
    /// </summary>
    public static class ButtonText
    {
        public static string REFRESH => ConstantStringLocalizer.Localize("Refresh");
        public static string EDIT => ConstantStringLocalizer.Localize("Edit");
        public static string DELETE => ConstantStringLocalizer.Localize("Delete");
        public static string ADD => ConstantStringLocalizer.Localize("Add");
        public static string CREATE => ConstantStringLocalizer.Localize("Create");
        public static string EXPORT => ConstantStringLocalizer.Localize("Export to Excel");
        public static string IMPORT => ConstantStringLocalizer.Localize("Import from Excel");
        public static string ACTIONS => ConstantStringLocalizer.Localize("Actions");
        public static string SAVE => ConstantStringLocalizer.Localize("Save");
        public static string SAVECHANGES => ConstantStringLocalizer.Localize("Save Changes");
        public static string CANCEL => ConstantStringLocalizer.Localize("Cancel");
        public static string CLOSE => ConstantStringLocalizer.Localize("Close");
        public static string SEARCH => ConstantStringLocalizer.Localize("Search");
        public static string CLEAR => ConstantStringLocalizer.Localize("Clear");
        public static string RESET => ConstantStringLocalizer.Localize("Reset");
        public static string OK => ConstantStringLocalizer.Localize("OK");
        public static string CONFIRM => ConstantStringLocalizer.Localize("Confirm");
        public static string YES => ConstantStringLocalizer.Localize("Yes");
        public static string NO => ConstantStringLocalizer.Localize("No");
        public static string NEXT => ConstantStringLocalizer.Localize("Next");
        public static string PREVIOUS => ConstantStringLocalizer.Localize("Previous");
        public static string UPLOADING => ConstantStringLocalizer.Localize("Uploading...");
        public static string DOWNLOADING => ConstantStringLocalizer.Localize("Downloading...");
        public static string NOALLOWED => ConstantStringLocalizer.Localize("No Allowed");
        public static string SIGNINWITH => ConstantStringLocalizer.Localize("Sign in with {0}");
        public static string LOGOUT => ConstantStringLocalizer.Localize("Logout");
        public static string SIGNIN => ConstantStringLocalizer.Localize("Sign In");
        public static string Microsoft => ConstantStringLocalizer.Localize("Microsoft");
        public static string Facebook => ConstantStringLocalizer.Localize("Facebook");
        public static string Google => ConstantStringLocalizer.Localize("Google");
    }
}

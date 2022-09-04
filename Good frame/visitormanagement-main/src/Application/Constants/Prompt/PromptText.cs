using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Helper;

namespace CleanArchitecture.Blazor.Application.Constants
{
    public static class PromptText
    {
        public static string ADVANCEDSEARCH => ConstantStringLocalizer.Localize("Advanced Search");
        public static string ORDERBY => ConstantStringLocalizer.Localize("Order By");
        public static string ACTIONS => ConstantStringLocalizer.Localize("Actions");
        public static string SEARCH => ConstantStringLocalizer.Localize("Search");
        public static string CREATEAITEM => ConstantStringLocalizer.Localize("Create a new {0}");
        public static string EDITTHEITEM => ConstantStringLocalizer.Localize("Edit the {0}");
        public static string DELETETHEITEM => ConstantStringLocalizer.Localize("Delete the {0}");
        public static string DELETEITEMS => ConstantStringLocalizer.Localize("Delete selected items: {0}");
        public static string DELETECONFIRMATION => ConstantStringLocalizer.Localize("Are you sure you want to delete this item: {0}?");
        public static string DELETECONFIRMATIONWITHID => ConstantStringLocalizer.Localize("Are you sure you want to delete this item with Id: {0}?");
        public static string DELETECONFIRMWITHSELECTED => ConstantStringLocalizer.Localize("Are you sure you want to delete the selected items: {0}?");
        public static string NOMACHING => ConstantStringLocalizer.Localize("No matching records found");
        public static string LOADING => ConstantStringLocalizer.Localize("Loading...");

        public static string DELETECONFIRMATIONTITLE => ConstantStringLocalizer.Localize("Delete Confirmation");

        public static string LOGOUTCONFIRMATIONTITLE => ConstantStringLocalizer.Localize("Logout Confirmation");
        public static string LOGOUTCONFIRMATION => ConstantStringLocalizer.Localize("You are attempting to log out of application. Do you really want to log out?");

    }
}

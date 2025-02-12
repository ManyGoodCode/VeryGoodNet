using Blazor.Server.UI.Models.SideMenu;
using MudBlazor;

namespace Blazor.Server.UI.Services.Navigation;

public class MenuService : IMenuService
{
    private readonly List<MenuSectionModel> features = new List<MenuSectionModel>()
    {
        new MenuSectionModel
        {
            Title = "Application",
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    Title = "Deshbord",
                    Icon = Icons.Material.Filled.Dashboard,
                    Href = "/"
                },
                new()
                {
                    Title = "Employees",
                    Icon = Icons.Material.Filled.AssignmentInd,
                    Href = "/visitor/employees",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Visitors",
                    Icon = Icons.Material.Filled.DirectionsRun,
                    Href = "/visitor/visitors",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Visitor Request",
                    Icon = Icons.Material.Filled.PostAdd,
                    Href = "/visitor/visitorrequest",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Complete Visit Info",
                    Icon = Icons.Material.Filled.EditNote,
                    Href = "/visitor/completevisitinfo",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Pre-registers",
                    Icon = Icons.Material.Filled.Bookmarks,
                    Href = "/visitor/preregisters",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "My Visit Code",
                    Icon = Icons.Material.Filled.QrCode,
                    Href = "/visitor/mycode",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Pending Approval",
                    Icon = Icons.Material.Filled.AppRegistration,
                    Href = "/visitor/pendingapproval",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Pending Confirm",
                    Icon = Icons.Material.Filled.FactCheck,
                    Href = "/visitor/pendingconfirm",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Pending Checking",
                    Icon = Icons.Material.Filled.FollowTheSigns,
                    Href = "/visitor/pendingcheckin",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Check-in",
                    Icon = Icons.Material.Filled.Camera,
                    Href = "/visitor/checkin",
                    PageStatus = PageStatus.Completed
                },
                 new()
                {
                    Title = "Reports",
                    Icon = Icons.Material.Filled.LineAxis,
                    Href = "/visitor/histories",
                    PageStatus = PageStatus.ComingSoon
                },
                new()
                {
                    Title = "Histories",
                    Icon = Icons.Material.Filled.History,
                    Href = "/visitor/visitorhistories",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    IsParent = true,
                    Title = "Sites & Org",
                    Icon = Icons.Material.Filled.LocationCity,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Departments",
                            Href = "/visitor/departments",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Designations",
                            Href = "/visitor/designations",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Sites",
                            Href = "/visitor/sites",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Message Templates",
                            Href = "/visitor/messagetemplates",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Site Configuration",
                            Href = "/visitor/siteconfigurations",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Check-in Points",
                            Href = "/visitor/checkinpoints",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Devices",
                            Href = "/visitor/devices",
                            PageStatus = PageStatus.Completed
                        },
                    }
                }
            }
        },

        new MenuSectionModel
        {
            Title = "MANAGEMENT",
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    IsParent = true,
                    Title = "Authorization",
                    Icon = Icons.Material.Filled.ManageAccounts,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Users",
                            Href = "/indentity/users",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Roles",
                            Href = "/indentity/roles",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Profile",
                            Href = "/user/profile",
                            PageStatus = PageStatus.Completed
                        }
                    }
                },
                new()
                {
                    IsParent = true,
                    Title = "System",
                    Icon = Icons.Material.Filled.Devices,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {   new()
                        {
                            Title = "Picklist",
                            Href = "/system/picklist",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Audit Trails",
                            Href = "/system/audittrails",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Log",
                            Href = "/system/logs",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Jobs",
                            Href = "/hangfire/index",
                            PageStatus = PageStatus.ComingSoon
                        }
                    }
                }

            }
        }
    };


    private readonly List<MenuSectionModel> _guestfeatures = new List<MenuSectionModel>()
    {
        new MenuSectionModel
        {
            Title = "Application",
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    Title = "Deshbord",
                    Icon = Icons.Material.Filled.Dashboard,
                    Href = "/"
                },
                new()
                {
                    Title = "Complete Visit Info",
                    Icon = Icons.Material.Filled.EditNote,
                    Href = "/visitor/completevisitinfo",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "My Visit Code",
                    Icon = Icons.Material.Filled.QrCode,
                    Href = "/visitor/mycode",
                    PageStatus = PageStatus.Completed
                },
            }
        },

         
    };

    private readonly List<MenuSectionModel> _guardfeatures = new List<MenuSectionModel>()
    {
        new MenuSectionModel
        {
            Title = "Application",
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    Title = "Deshbord",
                    Icon = Icons.Material.Filled.Dashboard,
                    Href = "/"
                },
                new()
                {
                    Title = "Pending Checking",
                    Icon = Icons.Material.Filled.FollowTheSigns,
                    Href = "/visitor/pendingcheckin",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Check-in",
                    Icon = Icons.Material.Filled.Camera,
                    Href = "/visitor/checkin",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Reports",
                    Icon = Icons.Material.Filled.LineAxis,
                    Href = "/visitor/reports",
                    PageStatus = PageStatus.ComingSoon
                },
            }
        },


    };

    private readonly List<MenuSectionModel> _userfeatures = new List<MenuSectionModel>()
    {
        new MenuSectionModel
        {
            Title = "Application",
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    Title = "Deshbord",
                    Icon = Icons.Material.Filled.Dashboard,
                    Href = "/"
                },
                new()
                {
                    Title = "Employees",
                    Icon = Icons.Material.Filled.AssignmentInd,
                    Href = "/visitor/employees",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Visitors",
                    Icon = Icons.Material.Filled.DirectionsRun,
                    Href = "/visitor/visitors",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Visitor Request",
                    Icon = Icons.Material.Filled.PostAdd,
                    Href = "/visitor/visitorrequest",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Pre-registers",
                    Icon = Icons.Material.Filled.Bookmarks,
                    Href = "/visitor/preregisters",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Pending Approval",
                    Icon = Icons.Material.Filled.AppRegistration,
                    Href = "/visitor/pendingapproval",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Pending Confirm",
                    Icon = Icons.Material.Filled.FactCheck,
                    Href = "/visitor/pendingconfirm",
                    PageStatus = PageStatus.Completed
                },
                new()
                {
                    Title = "Reports",
                    Icon = Icons.Material.Filled.LineAxis,
                    Href = "/visitor/histories",
                    PageStatus = PageStatus.ComingSoon
                },
                new()
                {
                    Title = "Histories",
                    Icon = Icons.Material.Filled.History,
                    Href = "/visitor/visitorhistories",
                    PageStatus = PageStatus.Completed
                }
                
            }
        },
    };
    public IEnumerable<MenuSectionModel> AllFeatures => features;
    public IEnumerable<MenuSectionModel> GuestFeatures => _guestfeatures;
    public IEnumerable<MenuSectionModel> GuardFeatures => _guardfeatures;
    public IEnumerable<MenuSectionModel> UserFeatures => _userfeatures;
}

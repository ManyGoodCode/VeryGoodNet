using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Constants.Permission;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using CleanArchitecture.Blazor.Infrastructure.Identity;
using HashidsNet;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence
{

    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            ApplicationRole administratorRole = new ApplicationRole(RoleConstants.AdministratorRole) { Description = "Admin Group" };
            ApplicationRole userRole = new ApplicationRole(RoleConstants.BasicRole) { Description = "Basic Group" };
            ApplicationRole guestRole = new ApplicationRole(RoleConstants.GuestRole) { Description = "Guest Group" };
            ApplicationRole guardRole = new ApplicationRole(RoleConstants.GuardRole) { Description = "Guard Group" };
            ApplicationRole usersRole = new ApplicationRole(RoleConstants.UserRole) { Description = "Guard Group" };
            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
                await roleManager.CreateAsync(userRole);
                await roleManager.CreateAsync(guestRole);
                await roleManager.CreateAsync(usersRole);
                await roleManager.CreateAsync(guardRole);
                IEnumerable<string> Permissions = GetAllPermissions();
                foreach (string permission in Permissions)
                {
                    await roleManager.AddClaimAsync(administratorRole, new Claim(ApplicationClaimTypes.Permission, permission));
                    if (permission.StartsWith("Permissions.Products"))
                        await roleManager.AddClaimAsync(userRole, new Claim(ApplicationClaimTypes.Permission, permission));
                }
            }

            ApplicationUser administrator = new ApplicationUser { UserName = "administrator", IsActive = true, Site = "Blazor", DisplayName = "Administrator", Email = "administrator@163.com", EmailConfirmed = true, ProfilePictureDataUrl = $"https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80" };
            ApplicationUser demo = new ApplicationUser { UserName = "Demo", IsActive = true, Site = "Blazor", DisplayName = "Demo", Email = "demo@163.com", EmailConfirmed = true, ProfilePictureDataUrl = $"https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80" };
            ApplicationUser user = new ApplicationUser { UserName = "Test", IsActive = true, Site = "Blazor", DisplayName = "Test", Email = "test@126.com", EmailConfirmed = true };
            ApplicationUser guest = new ApplicationUser { UserName = "Guest", IsActive = true, Site = "Blazor", DisplayName = "Guest", Email = "guest@126.com", EmailConfirmed = true };
            ApplicationUser guard = new ApplicationUser { UserName = "Guard", IsActive = true, Site = "Blazor", DisplayName = "Guard", Email = "guard@126.com", EmailConfirmed = true };
            if (userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await userManager.CreateAsync(administrator, RoleConstants.DefaultPassword);
                await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
                await userManager.CreateAsync(demo, RoleConstants.DefaultPassword);
                await userManager.AddToRolesAsync(demo, new[] { userRole.Name });
                await userManager.CreateAsync(user, RoleConstants.DefaultPassword);
                await userManager.AddToRolesAsync(user, new[] { usersRole.Name });
                await userManager.CreateAsync(guest, RoleConstants.DefaultPassword);
                await userManager.AddToRolesAsync(guest, new[] { guestRole.Name });
                await userManager.CreateAsync(guard, RoleConstants.DefaultPassword);
                await userManager.AddToRolesAsync(guard, new[] { guardRole.Name });
            }

        }
        private static IEnumerable<string> GetAllPermissions()
        {
            List<string> allPermissions = new List<string>();
            Type[] modules = typeof(Permissions).GetNestedTypes();

            foreach (Type module in modules)
            {
                string moduleName = string.Empty;
                string moduleDescription = string.Empty;

                FieldInfo[] fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (FieldInfo fi in fields)
                {
                    object? propertyValue = fi.GetValue(null);
                    if (propertyValue is not null)
                        allPermissions.Add(propertyValue.ToString());
                }
            }

            return allPermissions;
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            //Seed, if necessary
            if (!context.DocumentTypes.Any())
            {
                context.DocumentTypes.Add(new Domain.Entities.DocumentType() { Name = "Document", Description = "Document" });
                context.DocumentTypes.Add(new Domain.Entities.DocumentType() { Name = "PDF", Description = "PDF" });
                context.DocumentTypes.Add(new Domain.Entities.DocumentType() { Name = "Image", Description = "Image" });
                context.DocumentTypes.Add(new Domain.Entities.DocumentType() { Name = "Other", Description = "Other" });
                await context.SaveChangesAsync();

            }
            if (!context.KeyValues.Any())
            {
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Status", Value = "initialization", Text = "initialization", Description = "Status of workflow" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Status", Value = "processing", Text = "processing", Description = "Status of workflow" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Status", Value = "pending", Text = "pending", Description = "Status of workflow" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Status", Value = "finished", Text = "finished", Description = "Status of workflow" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Brand", Value = "Apple", Text = "Apple", Description = "Brand of production" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Brand", Value = "MI", Text = "MI", Description = "Brand of production" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Brand", Value = "Logitech", Text = "Logitech", Description = "Brand of production" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Brand", Value = "Linksys", Text = "Linksys", Description = "Brand of production" });

                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "EA", Text = "EA", Description = "Unit of product" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "KM", Text = "KM", Description = "Unit of product" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "PC", Text = "PC", Description = "Unit of product" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "KG", Text = "KG", Description = "Unit of product" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "ST", Text = "ST", Description = "Unit of product" });

                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Purpose", Value = "Meeting", Text = "Meeting", Description = "Visitor's Purpose" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Purpose", Value = "Interview", Text = "Interview", Description = "Visitor's Purpose" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Purpose", Value = "Conferences", Text = "Conferences", Description = "Visitor's Purpose" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Purpose", Value = "Working", Text = "Working", Description = "Visitor's Purpose" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Purpose", Value = "Others", Text = "Others", Description = "Visitor's Purpose" });

                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Gender", Value = "Male", Text = "Male", Description = "Visitor's Gender" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Gender", Value = "Female", Text = "Female", Description = "Visitor's Gender" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Gender", Value = "Unknown", Text = "Unknown", Description = "Visitor's Gender" });

                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "ActiveStatus", Value = "Active", Text = "Active", Description = "Active Status" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "ActiveStatus", Value = "Inactive", Text = "Inactive", Description = "Active Status" });

                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "DeviceStatus", Value = "Online", Text = "Online", Description = "Device Status" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "DeviceStatus", Value = "Offline", Text = "Offline", Description = "Device Status" });

                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "VisitorStatus", Value = "Pending Visitor", Text = "Pending Visitor", Description = "Visitor Status" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "VisitorStatus", Value = "Pending Approval", Text = "Pending Approval", Description = "Visitor Status" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "VisitorStatus", Value = "Pending Check-in", Text = "Pending Check-in", Description = "Visitor Status" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "VisitorStatus", Value = "Pending Check-out", Text = "Pending Check-out", Description = "Visitor Status" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "VisitorStatus", Value = "Pending Feedback", Text = "Pending Feedback", Description = "Visitor Status" });
                context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "VisitorStatus", Value = "Finished", Text = "Finished", Description = "Visitor Status" });
                await context.SaveChangesAsync();
            }
            if (!context.Products.Any())
            {
                context.Products.Add(new Domain.Entities.Product() { Brand = "Apple", Name = "IPhone 13 Pro", Description = "Apple iPhone 13 Pro smartphone. Announced Sep 2021. Features 6.1″ display, Apple A15 Bionic chipset, 3095 mAh battery, 1024 GB storage.", Unit = "EA", Price = 999.98m });
                context.Products.Add(new Domain.Entities.Product() { Brand = "MI", Name = "MI 12 Pro", Description = "Xiaomi 12 Pro Android smartphone. Announced Dec 2021. Features 6.73″ display, Snapdragon 8 Gen 1 chipset, 4600 mAh battery, 256 GB storage.", Unit = "EA", Price = 199.00m });
                context.Products.Add(new Domain.Entities.Product() { Brand = "Logitech", Name = "MX KEYS Mini", Description = "Logitech MX Keys Mini Introducing MX Keys Mini – a smaller, smarter, and mightier keyboard made for creators. Type with confidence on a keyboard crafted for efficiency, stability, and...", Unit = "PA", Price = 99.90m });
                await context.SaveChangesAsync();
            }
            if (!context.Departments.Any())
            {
                context.Departments.Add(new Domain.Entities.Department() { Name = "Service", Status = "Active" });
                context.Departments.Add(new Domain.Entities.Department() { Name = "IT", Status = "Active" });
                context.Departments.Add(new Domain.Entities.Department() { Name = "Marketing", Status = "Active" });
                context.Departments.Add(new Domain.Entities.Department() { Name = "Operation", Status = "Active" });
                await context.SaveChangesAsync();
            }
            if (!context.Designations.Any())
            {
                context.Designations.Add(new Domain.Entities.Designation() { Name = "HR Director", Status = "Active" });
                context.Designations.Add(new Domain.Entities.Designation() { Name = "Production Manager", Status = "Active" });
                context.Designations.Add(new Domain.Entities.Designation() { Name = "General HR Manager", Status = "Active" });
                context.Designations.Add(new Domain.Entities.Designation() { Name = "Chief Human Resource Officer", Status = "Active" });
                await context.SaveChangesAsync();
            }
            if (!context.Employees.Any())
            {
                var depId = context.Departments.First().Id;
                var desId = context.Designations.First().Id;
                context.Employees.Add(new Domain.Entities.Employee() { Name = "Mike Brown", Email = "Brown.Mike@gmail.com", PhoneNumber = "021-76888098", DepartmentId = depId, DesignationId = desId, Gender = "Female", About = "Nice" });
                await context.SaveChangesAsync();
            }

            if (!context.Sites.Any())
            {
                context.Sites.Add(new Domain.Entities.Site() { Name = "Kunshan Center", Address = "in southeastern Jiangsu province with Shanghai bordering its eastern border and Suzhou on its western boundary" });
                context.Sites.Add(new Domain.Entities.Site() { Name = "Suzhou CBD", Address = "Located in the heart of the western central business district (CBD) of Suzhou Industrial Park" });
                await context.SaveChangesAsync();
            }
            if (!context.CheckinPoints.Any())
            {
                var site = context.Sites.First();
                context.CheckinPoints.Add(new Domain.Entities.CheckinPoint() { Name = "Main Gate", Description = "changjiang road 180 no.", SiteId = site.Id });
                context.CheckinPoints.Add(new Domain.Entities.CheckinPoint() { Name = "Gate 1 South", Description = "heilongjiang road 210 no.", SiteId = site.Id });
                context.CheckinPoints.Add(new Domain.Entities.CheckinPoint() { Name = "Gate 2 South", Description = "heilongjiang road 212 no.", SiteId = site.Id });
                context.CheckinPoints.Add(new Domain.Entities.CheckinPoint() { Name = "Floor 2", Description = "R&D center", SiteId = site.Id });
                context.CheckinPoints.Add(new Domain.Entities.CheckinPoint() { Name = "SDB 9 reception", Description = "warehouse receiving space", SiteId = site.Id });
                await context.SaveChangesAsync();
            }
            if (!context.Devices.Any())
            {
                var checkpoints = context.CheckinPoints.ToList();
                context.Devices.Add(new Domain.Entities.Device() { Name = "Gate Machine 1", IPAddress = "192.168.100.120", Status = "Online", CheckinPointId = checkpoints[0].Id });
                context.Devices.Add(new Domain.Entities.Device() { Name = "Gate Machine 2", IPAddress = "192.168.100.121", Status = "Online", CheckinPointId = checkpoints[1].Id });
                context.Devices.Add(new Domain.Entities.Device() { Name = "Gate Machine 3", IPAddress = "192.168.100.122", CheckinPointId = checkpoints[2].Id });
                context.Devices.Add(new Domain.Entities.Device() { Name = "IPAD wifi", IPAddress = "192.168.100.123", CheckinPointId = checkpoints[3].Id });
                context.Devices.Add(new Domain.Entities.Device() { Name = "Face Id", IPAddress = "192.168.100.124", CheckinPointId = checkpoints[4].Id });
                await context.SaveChangesAsync();
            }
            if (!context.Visitors.Any())
            {
                var siteid = context.Sites.First().Id;
                var depId = context.Departments.First().Id;
                var desId = context.Designations.First().Id;
                var empId = context.Employees.First().Id;
                var hashids = new Hashids("Blazor.net");
                var hash = hashids.EncodeLong(DateTime.Now.Ticks);
                context.Visitors.Add(new Domain.Entities.Visitor() { Name = "Json Mic", SiteId = siteid, Email = "Json.mic@gmail.com", PhoneNumber = "021-76888098", IdentificationNo = "3205830000000001", DesignationId = desId, Gender = "Female", Purpose = "Meeting", Comment = "have a meeting...", Address = "China", PrivacyPolicy = true, Promise = true, ExpectedDate = DateTime.Now, ExpectedTime = new TimeSpan(12, 0, 0), CompanyName = "Google Inc.", EmployeeId = empId, PassCode = hash });
                await context.SaveChangesAsync();
            }
        }
    }
}

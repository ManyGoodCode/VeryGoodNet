using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public ApplicationRole() : base()
        {
            RoleClaims = new HashSet<ApplicationRoleClaim>();
            UserRoles = new HashSet<ApplicationUserRole>();
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
            RoleClaims = new HashSet<ApplicationRoleClaim>();
            UserRoles = new HashSet<ApplicationUserRole>();
        }
    }
}

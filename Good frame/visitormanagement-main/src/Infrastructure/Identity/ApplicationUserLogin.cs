using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Identity
{
    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser User { get; set; } = default!;
    }
}

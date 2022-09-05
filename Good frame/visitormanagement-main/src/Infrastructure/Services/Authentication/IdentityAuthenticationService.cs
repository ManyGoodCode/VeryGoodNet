using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CleanArchitecture.Blazor.Application.Common.Security;
using System.Text;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using CleanArchitecture.Blazor.Infrastructure.Constants.LocalStorage;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CleanArchitecture.Blazor.Infrastructure.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.IO;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication
{
    public class IdentityAuthenticationService : AuthenticationStateProvider, IAuthenticationService
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly ProtectedLocalStorage protectedLocalStorage;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private const string KEY = "Basic";

        public IdentityAuthenticationService(
            ProtectedLocalStorage protectedLocalStorage,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager
            )
        {
            this.protectedLocalStorage = protectedLocalStorage;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            try
            {
                var storedClaimsIdentity = await protectedLocalStorage.GetAsync<string>(LocalStorage.CLAIMSIDENTITY);
                if (storedClaimsIdentity.Success && storedClaimsIdentity.Value is not null)
                {
                    byte[] buffer = Convert.FromBase64String(storedClaimsIdentity.Value);
                    using (MemoryStream deserializationStream = new MemoryStream(buffer))
                    {
                        ClaimsIdentity identity = new ClaimsIdentity(new BinaryReader(deserializationStream, Encoding.UTF8));
                        principal = new ClaimsPrincipal(identity);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new AuthenticationState(principal);
        }

        private async Task<ClaimsIdentity> createIdentityFromApplicationUser(ApplicationUser user)
        {
            ClaimsIdentity result = new ClaimsIdentity(KEY);
            result.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            if (!string.IsNullOrEmpty(user.UserName))
            {
                result.AddClaims(new[]
                {
                 new Claim(ClaimTypes.Name, user.UserName)
                });
            }
            if (!string.IsNullOrEmpty(user.Site))
            {
                result.AddClaims(new[] {
                new Claim(ClaimTypes.Locality, user.Site)
            });
            }
            if (user.SiteId is not null)
            {
                result.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.SiteId, user.SiteId.ToString())
            });
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                result.AddClaims(new[] {
                new Claim(ClaimTypes.Email, user.Email)
            });
            }
            if (!string.IsNullOrEmpty(user.ProfilePictureDataUrl))
            {
                result.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl)
            });
            }
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                result.AddClaims(new[] {
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            });
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                result.AddClaims(new[] {
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
            });
            }
            if (!string.IsNullOrEmpty(user.Department))
            {
                result.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.Department, user.Department)
            });
            }
            if (!string.IsNullOrEmpty(user.Designation))
            {
                result.AddClaims(new[]
                {
                new Claim(ApplicationClaimTypes.Designation, user.Designation)
            });
            }

            IList<string> roles = await userManager.GetRolesAsync(user);
            foreach (string rolename in roles)
            {
                ApplicationRole role = await roleManager.FindByNameAsync(rolename);
                IList<Claim> claims = await roleManager.GetClaimsAsync(role);
                foreach (Claim claim in claims)
                {
                    result.AddClaim(claim);
                }

                result.AddClaims(new[] {
                new Claim(ClaimTypes.Role, rolename) });

            }
            return result;
        }


        public async Task<bool> Login(LoginFormModel request)
        {
            await semaphore.WaitAsync();
            try
            {
                var user = await userManager.FindByNameAsync(request.UserName);
                var valid = await userManager.CheckPasswordAsync(user, request.Password);
                if (valid)
                {

                    var identity = await createIdentityFromApplicationUser(user);
                    using (var memoryStream = new MemoryStream())
                    using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                    {
                        identity.WriteTo(binaryWriter);
                        var base64 = Convert.ToBase64String(memoryStream.ToArray());
                        await protectedLocalStorage.SetAsync(LocalStorage.CLAIMSIDENTITY, base64);
                    }

                    await protectedLocalStorage.SetAsync(LocalStorage.USERID, user.Id);
                    await protectedLocalStorage.SetAsync(LocalStorage.USERNAME, user.UserName);
                    if (user.Site is not null)
                    {
                        await protectedLocalStorage.SetAsync(LocalStorage.SITE, user.Site);
                    }
                    if (user.SiteId is not null)
                    {
                        await protectedLocalStorage.SetAsync(LocalStorage.SITEID, user.SiteId);
                    }

                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
                }

                return valid;
            }
            finally
            {
                semaphore.Release();
            }
        }
        public async Task<bool> ExternalLogin(string provider, string userName, string name, string accessToken)
        {
            await semaphore.WaitAsync();
            try
            {
                ApplicationUser user = await userManager.FindByNameAsync(userName);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        EmailConfirmed = true,
                        IsActive = true,
                        IsLive = true,
                        UserName = userName,
                        Email = userName.Any(x => x == '@') ? userName : $"{userName}@{provider}.com",
                        Site = provider,
                        DisplayName = name,
                    };

                    IdentityResult result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return false;
                    }
                    if (user.Email.ToLower().Contains("voith.com"))
                    {
                        await userManager.AddToRoleAsync(user, RoleConstants.UserRole);
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, RoleConstants.GuestRole);
                    }

                }

                ClaimsIdentity identity = await createIdentityFromApplicationUser(user);
                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    identity.WriteTo(binaryWriter);
                    string base64 = Convert.ToBase64String(memoryStream.ToArray());
                    await protectedLocalStorage.SetAsync(LocalStorage.CLAIMSIDENTITY, base64);
                }

                await protectedLocalStorage.SetAsync(LocalStorage.USERID, user.Id);
                await protectedLocalStorage.SetAsync(LocalStorage.USERNAME, user.UserName);
                if (user.Site is not null)
                {
                    await protectedLocalStorage.SetAsync(LocalStorage.SITE, user.Site);
                }
                if (user.SiteId is not null)
                {
                    await protectedLocalStorage.SetAsync(LocalStorage.SITEID, user.SiteId);
                }

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }
        public async Task Logout()
        {
            await protectedLocalStorage.DeleteAsync(LocalStorage.CLAIMSIDENTITY);
            await protectedLocalStorage.DeleteAsync(LocalStorage.USERID);
            await protectedLocalStorage.DeleteAsync(LocalStorage.USERNAME);
            await protectedLocalStorage.DeleteAsync(LocalStorage.SITE);
            await protectedLocalStorage.DeleteAsync(LocalStorage.SITEID);
            ClaimsPrincipal principal = new ClaimsPrincipal();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
    }
}

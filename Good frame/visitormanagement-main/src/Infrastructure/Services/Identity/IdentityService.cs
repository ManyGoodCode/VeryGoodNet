using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity
{

    public class IdentityService : IIdentityService
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly IServiceProvider serviceProvider;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IOptions<AppConfigurationSettings> appConfig;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory;
        private readonly IAuthorizationService authorizationService;
        private readonly IStringLocalizer<IdentityService> localizer;

        public IdentityService(
            IServiceProvider serviceProvider,
            IOptions<AppConfigurationSettings> appConfig,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService,
            IStringLocalizer<IdentityService> localizer)
        {
            this.serviceProvider = serviceProvider;
            this.userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            this.roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            this.appConfig = appConfig;
            this.userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            this.authorizationService = authorizationService;
            this.localizer = localizer;
        }

        public async Task<string?> GetUserNameAsync(string userId)
        {
            await semaphore.WaitAsync();
            try
            {
                ApplicationUser? user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
                return user?.UserName;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
            };

            IdentityResult? result = await userManager.CreateAsync(user, password);
            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            ApplicationUser? user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            return await userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            ApplicationUser? user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            ClaimsPrincipal? principal = await userClaimsPrincipalFactory.CreateAsync(user);
            AuthorizationResult? result = await authorizationService.AuthorizeAsync(principal, policyName);
            return result.Succeeded;
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            ApplicationUser user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            IdentityResult? result = await userManager.DeleteAsync(user);
            return result.ToApplicationResult();
        }

        public async Task<IDictionary<string, string>> FetchUsers(string roleName)
        {
            Dictionary<string, string?>? result = await userManager.Users
                 .Where(x => x.UserRoles.Where(y => y.Role.Name == roleName).Any())
                 .Include(x => x.UserRoles)
                 .ToDictionaryAsync(x => x.UserName, y => y.DisplayName);
            return result;
        }

        public async Task<Result<TokenResponse>> LoginAsync(TokenRequest request)
        {
            ApplicationUser? user = await userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return await Result<TokenResponse>.FailureAsync(new string[]
                {
                    localizer["User Not Found."]
                });
            }
            if (!user.IsActive)
            {
                return await Result<TokenResponse>.FailureAsync(new string[]
                {
                    localizer["User Not Active. Please contact the administrator."]
                });
            }
            if (!user.EmailConfirmed)
            {
                return await Result<TokenResponse>.FailureAsync(new string[]
                {
                    localizer["E-Mail not confirmed."]
                });
            }

            bool passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                return await Result<TokenResponse>.FailureAsync(new string[]
                {
                    localizer["Invalid Credentials."]
                });
            }

            user.RefreshToken = GenerateRefreshToken();
            DateTime TokenExpiryTime = DateTime.Now.AddDays(7);
            if (request.RememberMe)
            {
                TokenExpiryTime = DateTime.Now.AddYears(1);
            }
            user.RefreshTokenExpiryTime = TokenExpiryTime;
            await userManager.UpdateAsync(user);

            var token = await GenerateJwtAsync(user);
            var response = new TokenResponse { Token = token, RefreshTokenExpiryTime = TokenExpiryTime, RefreshToken = user.RefreshToken, ProfilePictureDataUrl = user.ProfilePictureDataUrl };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            if (request is null)
            {
                return await Result<TokenResponse>.FailureAsync(new string[]
                {
                    localizer["Invalid Client Token."]
                });
            }

            ClaimsPrincipal userPrincipal = GetPrincipalFromExpiredToken(request.Token);
            string userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return await Result<TokenResponse>.FailureAsync(new string[]
                {
                    localizer["User Not Found."]
                });
            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return await Result<TokenResponse>.FailureAsync(new string[]
                {
                    localizer["Invalid Client Token."]
                });

            string token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await userManager.UpdateAsync(user);

            TokenResponse response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private async Task<string> GenerateJwtAsync(ApplicationUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }
        private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var thisRole = await roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await roleManager.GetClaimsAsync(thisRole);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Locality, user.Site),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.DisplayName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

            return claims;
        }
        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddDays(2),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.Value.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(localizer["Invalid token"]);
            }

            return principal;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(appConfig.Value.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        public async Task UpdateLiveStatus(string userId, bool isLive)
        {
            await semaphore.WaitAsync();
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user is not null && user.IsLive != isLive)
                {
                    user.IsLive = isLive;
                    await userManager.UpdateAsync(user);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}

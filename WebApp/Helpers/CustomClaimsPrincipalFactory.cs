using System.Security.Claims;
using BusinessLogic.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WebApp.Helpers
{
    public class CustomClaimsPrincipalFactory(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IOptions<IdentityOptions> options)
        : UserClaimsPrincipalFactory<AppUser, AppRole>(userManager, roleManager, options)
    {
        private readonly UserManager<AppUser> _userManger = userManager;

        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);
            var roles = await _userManger.GetRolesAsync(user);
            ((ClaimsIdentity)principal.Identity!).AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName ?? ""),
                new Claim("Email",user.Email ?? ""),
                new Claim("FullName",user.FullName),
                new Claim("PhoneNumber",user.PhoneNumber ?? ""),
                new Claim("Avatar",user.AvatarUrl ?? string.Empty),
                new Claim("Roles",string.Join(";",roles)),
                new Claim("UserId",user.Id),
                new Claim("UserName",user.UserName ?? string.Empty),
                new Claim("MemberStatus", user.MemberStatus.ToString()),
                new Claim("CompanyName", user.CompanyName ?? ""),
                new Claim("Address", user.Address ?? ""),
            });
            return principal;
        }
    }
}
using System.Security.Claims;
using BusinessLogic.Services.Common;

namespace WebApp.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId")!;
            UserName = httpContextAccessor.HttpContext?.User?.FindFirstValue("UserName")!;
            FullName = httpContextAccessor.HttpContext?.User?.FindFirstValue("FullName")!;
            Email = httpContextAccessor.HttpContext?.User?.FindFirstValue("Email")!;
            PhoneNumber = httpContextAccessor.HttpContext?.User?.FindFirstValue("PhoneNumber")!;
            RoleName = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)!;
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList()!;
            Origin = httpContextAccessor.HttpContext?.Request.Headers["Origin"]!;
        }

        public string UserId { get; }
        public string UserName { get; }
        public string RoleName { get; }
        public string Email { get; }
        public string FullName { get; }
        public string PhoneNumber { get; }
        public string Origin { get; }

        public List<KeyValuePair<string, string>> Claims { get; set; }
    }
}
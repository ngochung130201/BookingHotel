using BusinessLogic.Constants.Application;
using BusinessLogic.Constants.Role;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace WebApp.Authorization
{
    /// <summary>
    /// initial construct
    /// </summary>
    /// <param name="roleService"></param>
    public class BaseResourceAuthorizationHandler(IRoleService roleService) : AuthorizationHandler<OperationAuthorizationRequirement, string>
    {
        /// <summary>
        /// Handle get authorization
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, string resource)
        {
            var roles = ((ClaimsIdentity)context.User.Identity!).Claims.FirstOrDefault(x => x.Type == ApplicationConstants.UserClaims.Roles);
            if (roles != null)
            {
                var listRole = roles.Value.Split(";");
                var hasPermission = await roleService.CheckPermission(resource, requirement.Name, listRole);
                if (hasPermission || listRole.Contains(RoleConstants.Admin))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
        }
    }
}
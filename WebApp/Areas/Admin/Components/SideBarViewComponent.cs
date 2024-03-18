using System.Security.Claims;
using BusinessLogic.Constants.Role;
using BusinessLogic.Dtos.Function;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Extensions;

namespace WebApp.Areas.Admin.Components
{
    public class SideBarViewComponent : ViewComponent
    {
        private readonly IFunctionService _functionService;

        /// <summary>
        /// Initial Construct
        /// </summary>
        /// <param name="functionService"></param>
        public SideBarViewComponent(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        /// <summary>
        /// Side bar View component
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var roles = ((ClaimsPrincipal)User).GetSpecificClaim("Roles");
            List<FunctionResponse> result;
            if (roles.Split(";").Contains(RoleConstants.Admin))
            {
                var functions = await _functionService.GetAll();
                result = functions.Data.OrderBy(x => x.SortOrder).ToList();
            }
            else
            {
                var functions = await _functionService.GetAllWithRole(roles);
                result = functions.Data;
            }
            return View(result);
        }
    }
}
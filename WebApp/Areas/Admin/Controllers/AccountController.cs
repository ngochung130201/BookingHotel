using BusinessLogic.Dtos.Account;
using BusinessLogic.Services;
using BusinessLogic.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Areas.Admin.Controllers
{
    public class AccountController(IUserService userService) : AdminBaseController
    {
        public async Task<IActionResult> Index(string userId)
        {
            var result = await userService.GetByIdAsync(userId);
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfileUser(AccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            var result = await userService.UpdateProfile(request);
            return Json(result);
        }
    }
}
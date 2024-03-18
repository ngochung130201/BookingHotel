using BusinessLogic.Constants.Role;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Admin.Models.Login;
using WebApp.Extensions;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController(SignInManager<AppUser> signInManager) : Controller
    {
        /// <summary>
        /// View Login
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(string? returnUrl = null)
        {
            await signInManager.SignOutAsync();
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roles = User.GetSpecificClaim("Roles");
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded && roles != RoleConstants.User)
                {
                    return new OkObjectResult(await Result.SuccessAsync());
                }

                return new ObjectResult(await Result.FailAsync("UserName or password invalid!"));
            }

            // If we got this far, something failed, redisplay form
            return View("Index");
        }

        /// <summary>
        /// Function Logout
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        /// <summary>
        /// Access Denied
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
using BusinessLogic.Dtos.Home;
using BusinessLogic.Dtos.News;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Services;
using BusinessLogic.Services.Identity;
using BusinessLogic.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController(INewsService newsService, 
                                ITokenService tokenService,
                                IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
                                SignInManager<AppUser> signInManager,
                                UserManager<AppUser> userManager) : Controller
    {
        /// <summary>
        /// Home
        /// </summary>
        /// <returns></returns>
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var result = new HomeResponse();
            var resultNews = await newsService.GetPagination(new NewRequest
            {
                PageNumber = 1,
                PageSize = 3,
                Status = true
            });

            result.News = resultNews.Data;
            return View(result);
        }
        /// <summary>
        /// my account
        /// </summary>
        /// <returns></returns>
        [Route("my-account")]
        [Authorize]
        public async Task<IActionResult> MyAccount()
        {
            var result = new MyAccountResponse();
            var currentUserIdentity = User.Identity!;
            if (currentUserIdentity.IsAuthenticated)
            {
                var userWithSameEmail = await userManager.FindByEmailAsync(currentUserIdentity.Name!);

                result.CompanyName = userWithSameEmail!.CompanyName;
                result.Address = userWithSameEmail.Address;
                result.AvatarUrl = userWithSameEmail.AvatarUrl;
                result.Email = userWithSameEmail.Email!;
                result.FullName = userWithSameEmail.FullName;
                result.PhoneNumber = userWithSameEmail.PhoneNumber;
            }

            return View(result);
        }

        /// <summary>
        /// save my account
        /// </summary>
        /// <returns></returns>
        [Route("save-my-account")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveEntityAccount(MyAccountResponse request)
        {
            var result = await tokenService.UpdateAsync(request);
            if (result.Succeeded)
            {
                var principal = await userClaimsPrincipalFactory.CreateAsync(result.Data!);
                await signInManager.SignOutAsync();
                await signInManager.SignInAsync(result.Data!, isPersistent: false);
            }
            return Json(result);
        }

        /// <summary>
        /// save my account
        /// </summary>
        /// <returns></returns>
        [Route("change-password")]
        [HttpPost]
        public async Task<IActionResult> SaveEntityAccount(ChangePasswordRequest request)
        {
            var currentUserIdentity = User.Identity!;
            if (currentUserIdentity.IsAuthenticated)
            {
                var result = await tokenService.ChangePasswordAsync(request, currentUserIdentity.Name!);
                return Json(result);
            }

            return Json(Result.FailAsync("Change Password Fail"));
        }
    }
}

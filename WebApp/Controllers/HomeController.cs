using BusinessLogic.Dtos.Home;
using BusinessLogic.Dtos.News;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Services;
using BusinessLogic.Services.Identity;
using BusinessLogic.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace WebApp.Controllers
{
    public class HomeController(INewsService newsService,
                                IRoomTypesService roomTypesService, 
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

            var resultRoomTypes = await roomTypesService.GetPagination(new RoomTypesRequest
            {
                PageNumber = 1,
                PageSize = 4,
            });

            result.News = resultNews.Data;
            result.RoomTypes = resultRoomTypes.Data;
            result.LanguageType = CultureInfo.CurrentCulture.Name;

            return View(result);
        }

        /// <summary>
        /// Set language
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
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
                if (userWithSameEmail == null)
                {
                    if (currentUserIdentity.Name == "superadmin")
                    {
                        userWithSameEmail = await userManager.FindByEmailAsync("superadmin@gmail.com");
                    }
                }

                if (userWithSameEmail != null)
                {
                    result.CompanyName = userWithSameEmail!.CompanyName;
                    result.Address = userWithSameEmail.Address;
                    result.AvatarUrl = userWithSameEmail.AvatarUrl;
                    result.Email = userWithSameEmail.Email!;
                    result.FullName = userWithSameEmail.FullName;
                    result.PhoneNumber = userWithSameEmail.PhoneNumber;
                }
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
            var email = string.Empty;
            if (currentUserIdentity.IsAuthenticated)
            {
                if (currentUserIdentity.Name == "superadmin") email = "superadmin@gmail.com";
                else email = currentUserIdentity.Name;
                var result = await tokenService.ChangePasswordAsync(request, email);
                return Json(result);
            }

            return Json(Result.FailAsync("Change Password Fail"));
        }
    }
}

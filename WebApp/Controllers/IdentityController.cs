using BusinessLogic.Dtos.Identity.Requests;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Services.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using ForgotPasswordRequest = BusinessLogic.Dtos.Identity.Requests.ForgotPasswordRequest;
using RegisterRequest = BusinessLogic.Dtos.Identity.Requests.RegisterRequest;
using ResetPasswordRequest = BusinessLogic.Dtos.Identity.Requests.ResetPasswordRequest;
using BusinessLogic.Constants.User;

namespace WebApp.Controllers
{
    [Route("identity")]
    public class IdentityController(ITokenService tokenService, SignInManager<AppUser> signInManager) : Controller
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        [Route("login")]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View(new TokenMemberRequest());
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(TokenMemberRequest model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Check if user is inactive
                var user = await signInManager.UserManager.FindByEmailAsync(model.Email);
                if (user is { EmailConfirmed: false } or { IsActive: false })
                {
                    ModelState.AddModelError(string.Empty, "Username or password invalid!");
                    return View(model);
                }
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Redirect(returnUrl ?? "/");
                }

                ModelState.AddModelError(string.Empty, "Username or password invalid!");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <returns></returns>
        [Route("forgot-password")]
        public IActionResult ForgotPassword()
        {
            var model = new ForgotPasswordRequest();
            return View(model);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            var origin = Request.Headers["origin"];
            var result = await tokenService.ForgotPasswordAsync(request, origin!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Messages)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(request);
        }

        /// <summary>
        /// Confirm Email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code)
        {
            var result = await tokenService.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return View();
            }
            return RedirectToAction("SignUp");
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [Route("reset-password")]
        public IActionResult ResetPassword(string? token = null, string? email = null)
        {
            var model = new ResetPasswordRequest()
            {
                Token = token ?? string.Empty,
                Email = email ?? string.Empty,
                ResetFlag = false
            };
            return View(model);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            request.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await tokenService.ResetPasswordAsync(request);
            if (!result.Succeeded)
            {
                foreach (var error in result.Messages)
                {
                    ModelState.AddModelError("", error);
                }

                request.ResetFlag = false;
            }
            return View(request);
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <returns></returns>
        [Route("register")]
        public IActionResult SignUp()
        {
            return View(new RegisterRequest());
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            var origin = Request.Headers["origin"];
            var result = await tokenService.RegisterAsync(request, origin!);
            if (result.Succeeded)
            {
                return RedirectToAction("EmailVerify");
            }

            foreach (var error in result.Messages)
            {
                ModelState.AddModelError("", error);
            }
            return View(request);
        }

        /// <summary>
        /// logout post
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        /// LoginWithGoogle
        /// </summary>
        /// <returns></returns>
        [Route("loginWithGoogle")]
        public async Task LoginWithGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        /// <summary>
        /// GoogleResponse
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var claims = result.Principal!.Identities.FirstOrDefault()!.Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value,
                }).ToList();

                if (claims.Count >= 5)
                {
                    var addUser = new RegisterRequest
                    {
                        Email = claims[4].Value,
                        FirstName = claims[2].Value,
                        LastName = claims[3].Value,
                        Password = UserConstants.DefaultPassword,
                    };

                    var user = await tokenService.RegisterWithGoogleAsync(addUser);
                    if (user.Succeeded)
                    {
                        if (user.Data is { EmailConfirmed: false } or { IsActive: false })
                        {
                            ModelState.AddModelError(string.Empty, "Your account is locked!");
                            return RedirectToAction("Login", "Identity");
                        }
                        await signInManager.SignInAsync(user.Data!, isPersistent: false);
                        return Redirect("/");
                    }

                    ModelState.AddModelError(string.Empty, "Login unsuccessful!");
                    return RedirectToAction("Login", "Identity");
                }
            }
            ModelState.AddModelError(string.Empty, "Login unsuccessful!");
            return RedirectToAction("Login", "Identity");
        }

        /// <summary>
        /// Email verify
        /// </summary>
        /// <returns></returns>
        [Route("email-verify")]
        public IActionResult EmailVerify()
        {
            return View();
        }
    }
}
using BusinessLogic.Constants.Role;
using BusinessLogic.Dtos.Identity.Requests;
using BusinessLogic.Dtos.Mail;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Common;
using BusinessLogic.Wrapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Dtos.Home;
using BusinessLogic.Enums;

namespace BusinessLogic.Services.Identity
{
    public interface ITokenService
    {
        Task<IResult> RegisterAsync(RegisterRequest request, string origin);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<IResult<AppUser?>> RegisterWithGoogleAsync(RegisterRequest request);

        Task<IResult<AppUser?>> UpdateAsync(MyAccountResponse request);
        Task<IResult> ChangePasswordAsync(Dtos.Home.ChangePasswordRequest request, string email);

    }

    public class IdentityService( UserManager<AppUser> userManager, IEmailService mailService) : ITokenService
    {
        public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
        {
            var user = new AppUser
            {
                Email = request.Email,
                FullName = request.FirstName + " " + request.LastName,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                EmailConfirmed = false,
                MemberStatus = MemberStatus.Trial
            };

            var userWithSameEmail = await userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, RoleConstants.Customer);
                    // Send email to user for confirmation
                    var verificationUri = await SendVerificationEmail(user, origin);
                    var mailRequest = new MailRequest
                    {
                        From = "admin@nineplus.com.vn",
                        To = user.Email,
                        Body = $"Please confirm your account by <a href='{verificationUri}'>clicking here</a>.",
                        Subject = "Confirm Registration"
                    };
                    BackgroundJob.Enqueue(() => mailService.SendAsync(mailRequest));
                    return await Result<string>.SuccessAsync(user.Id,
                        $"User with email {user.Email} Registered. Please check your Mailbox to verify!");
                }

                return await Result.FailAsync(result.Errors.Select(a => a.Description.ToString()).ToList());
            }

            return await Result.FailAsync($"Email {request.Email} is already registered.");
        }

        private async Task<string> SendVerificationEmail(AppUser user, string origin)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            const string route = "identity/confirm-email";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }

        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result<string>.FailAsync(MessageConstants.NotFound);
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return await Result<string>.SuccessAsync(user.Id, $"Account Confirmed for {user.Email}.");
            }

            throw new ApiException($"An error occurred while confirming {user.Email}");
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return await Result.FailAsync("An Error has occurred!");
            }
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            const string route = "identity/reset-password";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code) + "&Email=" + request.Email;
            var mailRequest = new MailRequest
            {
                Body = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(passwordResetUrl)}'>clicking here</a>.",
                Subject = "Reset Password",
                To = request.Email
            };
            BackgroundJob.Enqueue(() => mailService.SendAsync(mailRequest));
            return await Result.SuccessAsync("Password Reset Mail has been sent to your authorized Email.");
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return await Result.FailAsync("An Error has occured!");
            }

            var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync("Password Reset Successful!");
            }

            return await Result.FailAsync("An Error has occured!");
        }

        public async Task<IResult<AppUser?>> RegisterWithGoogleAsync(RegisterRequest request)
        {
            var user = new AppUser
            {
                Email = request.Email,
                FullName = request.FirstName + " " + request.LastName,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                EmailConfirmed = true
            };

            var userWithSameEmail = await userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, RoleConstants.Customer);
                    return await Result<AppUser>.SuccessAsync(user);
                }

                return await Result<AppUser>.FailAsync(result.Errors.Select(a => a.Description.ToString()).ToList());
            }

            return await Result<AppUser>.SuccessAsync(userWithSameEmail);
        }

        public async Task<IResult<AppUser?>> UpdateAsync(MyAccountResponse request)
        {
            var userWithSameEmail = await userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                userWithSameEmail.FullName = request.FullName!;
                userWithSameEmail.PhoneNumber = request.PhoneNumber;
                userWithSameEmail.AvatarUrl = request.AvatarUrl;
                userWithSameEmail.CompanyName = request.CompanyName;
                userWithSameEmail.Address = request.Address;
                var result = await userManager.UpdateAsync(userWithSameEmail);
                if (result.Succeeded)
                {
                    return await Result<AppUser>.SuccessAsync(userWithSameEmail);
                }
                return await Result<AppUser>.FailAsync();
            }
            return await Result<AppUser>.FailAsync();
        }

        public async Task<IResult> ChangePasswordAsync(Dtos.Home.ChangePasswordRequest request, string email)
        {
            var userWithSameEmail = await userManager.FindByEmailAsync(email);
            if (userWithSameEmail != null)
            {
                var passwordCheck = await userManager.CheckPasswordAsync(userWithSameEmail, request.CurrentPassword);
                if (!passwordCheck)
                {
                    return await Result.FailAsync("Old password is incorrect");
                }
                var code = await userManager.GeneratePasswordResetTokenAsync(userWithSameEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await userManager.ResetPasswordAsync(userWithSameEmail, token, request.Password);
                if (result.Succeeded)
                {
                    return await Result.SuccessAsync();
                }
                return await Result.FailAsync("Change Password Fail");
            }
            return await Result.FailAsync("User does not exist");
        }
    }
}
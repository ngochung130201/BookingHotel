using BusinessLogic.Dtos.Identity.Requests;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Wrapper;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services.Identity
{
    public interface IAccountService
    {
        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);
    }

    public class AccountService(
        UserManager<AppUser> userManager)
        : IAccountService
    {
        public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result.FailAsync("User Not Found.");
            }

            var identityResult = await userManager.ChangePasswordAsync(
                user,
                model.Password,
                model.NewPassword);
            var errors = identityResult.Errors.Select(e => e.Description.ToString()).ToList();
            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
        }
    }
}
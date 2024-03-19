using AutoMapper;
using BusinessLogic.Constants.Application;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Account;
using BusinessLogic.Dtos.Role;
using BusinessLogic.Dtos.User;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Enums;
using BusinessLogic.Exceptions;
using BusinessLogic.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace BusinessLogic.Services
{
    public interface IUserService
    {
        Task<PaginatedResult<UserResponse>> GetAllPaginationAsync(UserRequest request);

        Task<Result<UserDetailDto>> GetByIdAsync(string id);

        Task<IResult> AddAsync(UserDetailDto request);

        Task<IResult> UpdateAsync(UserDetailDto request);

        Task<IResult> DeleteAsync(string id);

        Task<IResult> ChangeUserStatusAsync(string id);

        Task<IResult> ChangeMemberStatusAsync(string id, MemberStatus memberStatus);

        Task<IResult<List<RoleResponse>>> GetAllRole();

        Task<IResult> ChangePasswordUser(ChangePasswordUser changePasswordUser);

        Task<IResult> UpdateProfile(AccountRequest request);
    }

    public class UserService(ApplicationDbContext dbContext, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager) : IUserService
    {
        public async Task<PaginatedResult<UserResponse>> GetAllPaginationAsync(UserRequest request)
        {
            var query = from u in dbContext.Users.Where(x => !x.IsDeleted && !x.UserName!.Equals("superadmin"))
                        join ur in dbContext.UserRoles
                            on u.Id equals ur.UserId
                        join r in dbContext.Roles.Where(x => string.IsNullOrEmpty(request.RoleName) || x.Name!.Equals(request.RoleName))
                            on ur.RoleId equals r.Id
                        where string.IsNullOrEmpty(request.Keyword) || u.FullName.ToLower().Contains(request.Keyword.ToLower()) || (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(request.Keyword.ToLower()))
                        select new UserResponse
                        {
                            Id = u.Id,
                            Email = u.Email,
                            FullName = u.FullName,
                            CreatedOn = u.CreatedOn,
                            AvatarUrl = u.AvatarUrl,
                            IsActive = u.IsActive,
                            EmailConfirmed = u.EmailConfirmed,
                            RoleName = r.Name,
                            MemberStatus = u.MemberStatus,
                            UserName = u.UserName,
                            DateOfBirth = u.DateOfBirth,
                            PhoneNumber = u.PhoneNumber
                        };

            var totalRecord = query.Count();

            var result = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            return PaginatedResult<UserResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<UserDetailDto>> GetByIdAsync(string id)
        {
            var result = (from u in dbContext.Users.Where(x => !x.IsDeleted && x.Id == id)
                          join ur in dbContext.UserRoles
                              on u.Id equals ur.UserId
                          join r in dbContext.Roles
                              on ur.RoleId equals r.Id
                          select new UserDetailDto()
                          {
                              Id = u.Id,
                              Email = string.IsNullOrEmpty(u.Email) ? string.Empty : u.Email,
                              FullName = u.FullName,
                              AvatarUrl = u.AvatarUrl,
                              IsActive = u.IsActive,
                              RoleName = r.Id,
                              DateOfBirth = u.DateOfBirth,
                              PhoneNumber = u.PhoneNumber,
                              Address = u.Address,
                              Roles = string.IsNullOrEmpty(r.Name) ? string.Empty : r.Name,
                          }).AsNoTracking().FirstOrDefaultAsync();
            return await Result<UserDetailDto>.SuccessAsync(result.Result ?? new UserDetailDto());
        }

        public async Task<IResult> UpdateAsync(UserDetailDto request)
        {
            var user = await dbContext.Users.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

            if (user == null) return await Result.FailAsync(MessageConstants.NotFound);

            var checkIsValidEmail = IsValidEmail(request.Email);
            if (!checkIsValidEmail)
            {
                return await Result.FailAsync(MessageConstants.IsInvalidEmail);
            }

            var checkExistEmail = await dbContext.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Email) && x.Email.Contains(request.Email) && x.Id != request.Id);
            if (checkExistEmail) return await Result.FailAsync(string.Format(MessageConstants.CheckExists, "Email"));

            user.Id = request.Id;
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.AvatarUrl = request.AvatarUrl;
            user.Address = request.Address;
            user.DateOfBirth = request.DateOfBirth;
            user.NormalizedUserName = request.Email.ToUpper();
            user.UserName = request.Email;
            user.NormalizedEmail = request.Email.ToUpper();
            user.Password = request.Password;
            dbContext.Users.Update(user);

            await dbContext.SaveChangesAsync();

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public async Task<IResult> DeleteAsync(string id)
        {
            var user = await dbContext.Users.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (user == null) return await Result.FailAsync(MessageConstants.NotFound);

            user.IsDeleted = true;
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
        }

        public async Task<IResult> ChangeUserStatusAsync(string id)
        {
            var user = await dbContext.Users.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (user == null) return await Result.FailAsync(MessageConstants.NotFound);

            user.IsActive = !user.IsActive;
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public async Task<IResult> ChangeMemberStatusAsync(string id, MemberStatus memberStatus)
        {
            var user = await dbContext.Users.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (user == null) return await Result.FailAsync(MessageConstants.NotFound);

            user.MemberStatus = memberStatus;
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public async Task<IResult> AddAsync(UserDetailDto request)
        {
            var checkIsValidEmail = IsValidEmail(request.Email);
            if (!checkIsValidEmail)
            {
                return await Result.FailAsync(MessageConstants.IsInvalidEmail);
            }
            var checkEmailExists = await dbContext.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Email) && x.Email.Contains(request.Email) && !x.IsDeleted);
            if (checkEmailExists)
            {
                return await Result.FailAsync(string.Format(MessageConstants.CheckExists, "Email"));
            }
            var user = new AppUser
            {
                Email = request.Email,
                FullName = request.FullName,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.IsActive,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                AvatarUrl = request.AvatarUrl,
                DateOfBirth = request.DateOfBirth,
                MemberStatus = 0,
                Address = request.Address,
                Password = request.Password
            };

            var result = await userManager.CreateAsync(user, request.ConfirmPassword);
            if (result.Succeeded)
            {
                if (request.Roles.Any())
                {
                    var roleById = await roleManager.FindByIdAsync(request.Roles);
                    if (roleById != null && !string.IsNullOrEmpty(roleById.Name))
                    {
                        await userManager.AddToRoleAsync(user, roleById.Name);
                    }
                }
                return await Result<string>.SuccessAsync(user.Id, MessageConstants.AddSuccess);
            }
            throw new ApiException(MessageConstants.NotFound);
        }

        private static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        public async Task<IResult<List<RoleResponse>>> GetAllRole()
        {
            var result = await roleManager.Roles.Where(x => !x.IsDeleted && !string.IsNullOrEmpty(x.Name) && !x.Name.Equals("Admin"))
                .Select(a => new RoleResponse()
                {
                    Id = a.Id,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    Description = a.Description,
                    Name = string.IsNullOrEmpty(a.Name) ? string.Empty : a.Name
                }).ToListAsync();
            if (result.Any())
            {
                return Result<List<RoleResponse>>.Success(result);
            }
            return Result<List<RoleResponse>>.Fail(MessageConstants.NotFound);
        }

        public async Task<IResult> ChangePasswordUser(ChangePasswordUser changePasswordUser)
        {
            if (changePasswordUser is null)
            {
                return Result<string>.Fail(MessageConstants.NotFound);
            }
            var user = await userManager.Users.Where(x => x.Id == changePasswordUser.Id && !x.IsDeleted).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.Password == null) user.Password = string.Empty;
                var changePasswordResult = await userManager.ChangePasswordAsync(user, user.Password, changePasswordUser.ConfirmPasswordNew);

                if (changePasswordResult.Succeeded)
                {
                    user.Password = changePasswordUser.ConfirmPasswordNew;
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                    await signInManager.RefreshSignInAsync(user);
                    return await Result<string>.SuccessAsync(MessageConstants.AddSuccess);
                }
                else
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        return Result<string>.Fail(error.Description);
                    }
                }
            }
            return Result<string>.Fail(MessageConstants.NotFound);
        }

        public async Task<IResult> UpdateProfile(AccountRequest request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (user != null)
            {
                user.Id = request.Id;
                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                user.AvatarUrl = request.AvatarUrl;
                user.Address = request.Address;
                user.DateOfBirth = request.DateOfBirth;
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();

                // Change Password
                if (request.PasswordCurrent != null && request.PasswordNew != null)
                {
                    if (!request.PasswordNew.Equals(request.ConfirmPassword))
                    {
                        return Result<string>.Fail(MessageConstants.PasswordNotMatch);
                    }
                    var passwordHasher = new PasswordHasher<AppUser>();
                    if (user.PasswordHash == null) user.PasswordHash = string.Empty;
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.PasswordCurrent);
                    if (result == PasswordVerificationResult.Success)
                    {
                        IdentityResult identityResult = await userManager.ChangePasswordAsync(user, request.PasswordCurrent, request.PasswordNew);
                        if (identityResult.Succeeded)
                        {
                            user.Password = request.ConfirmPassword;
                            dbContext.Users.Update(user);
                            await dbContext.SaveChangesAsync();
                            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
                        }
                        else
                        {
                            return Result<string>.Fail(MessageConstants.ChangePasswordWasFail);
                        }
                    }
                    else
                    {
                        return Result<string>.Fail(MessageConstants.PasswordIsInCorrect);
                    }
                }
                return await Result<string>.SuccessAsync(MessageConstants.UpdateSuccess);
            }
            return Result<string>.Fail(MessageConstants.NotFound);
        }
    }
}
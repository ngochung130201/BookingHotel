using BusinessLogic.Dtos.Role;
using BusinessLogic.Dtos.User;
using BusinessLogic.Enums;
using BusinessLogic.Helpers;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace WebApp.Areas.Admin.Controllers
{
    public class UserController(IUserService userService) : AdminBaseController
    {
        public async Task<IActionResult> IndexAsync()
        {
            var result = await userService.GetAllRole();
            if(result.Data == null) { return View(new List<RoleResponse>()); }
            return View(result.Data);
        }

        /// <summary>
        /// Get User Pagination
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUserPaging(UserRequest request)
        {
            var result = await userService.GetAllPaginationAsync(request);
            return Json(result);
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await userService.GetByIdAsync(id);

            return Json(result);
        }
        /// <summary>
        /// Save Entity
        /// </summary>
        /// <param name="userDetailDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntity(UserDetailDto userDetailDto)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            if (userDetailDto.Id == "0")
            {
                var result = await userService.AddAsync(userDetailDto);
                return Json(result);
            }
            else
            {
                var result = await userService.UpdateAsync(userDetailDto);
                return Json(result);
            }
        }

        /// <summary>
        /// Change User Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangeUserStatus(string id)
        {
            var result = await userService.ChangeUserStatusAsync(id);
            return Json(result);
        }

        /// <summary>
        /// Change Member Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberStatus"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangeMemberStatus(string id, MemberStatus memberStatus)
        {
            var result = await userService.ChangeMemberStatusAsync(id, memberStatus);
            return Json(result);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await userService.DeleteAsync(id);
            return Json(result);
        }

        /// <summary>
        /// Change Password User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberStatus"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangePasswordUser(ChangePasswordUser changePasswordUser)
        {
            var result = await userService.ChangePasswordUser(changePasswordUser);
            return Json(result);
        }
    }
}

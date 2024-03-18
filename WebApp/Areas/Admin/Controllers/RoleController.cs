using BusinessLogic.Dtos.Function;
using BusinessLogic.Dtos.Role;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Areas.Admin.Controllers
{
    public class RoleController(IRoleService roleService, IFunctionService functionService) : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get User Pagination
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPaging(RoleRequest request)
        {
            var result = await roleService.GetAllPaginationAsync(request);
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
            var result = await roleService.GetByIdAsync(id);

            return Json(result);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntity(RoleDetailDto request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (string.IsNullOrEmpty(request.Id))
            {
                var result = await roleService.AddAsync(request);
                return Json(result);
            }
            else
            {
                var result = await roleService.UpdateAsync(request);
                return Json(result);
            }
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(string id)
        {
            var result = await roleService.DeleteAsync(id);
            return Json(result);
        }

        /// <summary>
        /// save permission
        /// </summary>
        /// <param name="listPermission"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SavePermission(List<FunctionWithRoleDto> listPermission, string roleId)
        {
            var result = await roleService.SavePermission(listPermission, roleId);
            return Ok(result);
        }

        /// <summary>
        /// get list menu for permission
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ListAllFunction(string roleId)
        {
            var result = await roleService.GetListFunctionWithRole(roleId);
            return Ok(result);
        }

        /// <summary>
        /// Get All Function
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllFunction()
        {
            var model = await functionService.GetAll();
            var rootFunctions = model.Data.Where(c => c.ParentId == null);
            var items = new List<FunctionResponse>();
            foreach (var function in rootFunctions)
            {
                //add the parent category to the item list
                items.Add(function);
                //now get all its children (separate Category in case you need recursion)
                GetByParentId(model.Data.ToList(), function, items);
            }
            return new ObjectResult(items);
        }

        private void GetByParentId(IEnumerable<FunctionResponse> allFunctions,
            FunctionResponse parent, IList<FunctionResponse> items)
        {
            var functionsEntities = allFunctions as FunctionResponse[] ?? allFunctions.ToArray();
            var subFunctions = functionsEntities.Where(c => c.ParentId == parent.FunctionId);
            foreach (var cat in subFunctions)
            {
                //add this category
                items.Add(cat);
                //recursive call in case your have a hierarchy more than 1 level deep
                GetByParentId(functionsEntities, cat, items);
            }
        }
    }
}

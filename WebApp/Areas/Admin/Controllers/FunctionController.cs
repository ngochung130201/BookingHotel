using BusinessLogic.Dtos.Function;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Areas.Admin.Controllers
{
    public class FunctionController(IFunctionService functionService) : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get Function Pagination
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await functionService.GetAll();
            return Json(result);
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetById(string functionId)
        {
            var result = await functionService.GetById(functionId);

            return Json(result);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntity(FunctionDetailDto request)
        {
            var isExist = await functionService.CheckExistedId(request.FunctionId);
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (!isExist.Data)
            {
                var result = await functionService.Add(request);
                return Json(result);
            }
            else
            {
                var result = await functionService.Update(request);
                return Json(result);
            }
        }

        /// <summary>
        /// Delete by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = await functionService.Delete(id);

            return Json(result);
        }

        /// <summary>
        /// Update parent id
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateParentId(string sourceId, string targetId, Dictionary<string, int> items)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (sourceId == targetId)
            {
                return new BadRequestResult();
            }

            var result = await functionService.UpdateParentId(sourceId, targetId, items);
            return Ok(result);
        }

        /// <summary>
        /// update order for menu
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReOrder(string sourceId, string targetId)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (sourceId == targetId)
            {
                return new BadRequestResult();
            }

            var result = await functionService.ReOrder(sourceId, targetId);
            return Ok(result);
        }
    }
}
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Dtos.FeedBacks;

namespace WebApp.Areas.Admin.Controllers
{
    public class FeedBackController(IFeedBackService feedBackService) : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get Pagination
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPaging(FeedBacksRequest request)
        {
            var result = await feedBackService.GetPagination(request);
            return Json(result);
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await feedBackService.GetById(id);

            return Json(result);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntity(FeedBacksDto request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (request.Id == 0)
            {
                var result = await feedBackService.Add(request);
                return Json(result);
            }
            else
            {
                var result = await feedBackService.Update(request);
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

            var result = await feedBackService.Delete(id);

            return Json(result);
        }
    }
}

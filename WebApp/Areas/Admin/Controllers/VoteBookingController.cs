using BusinessLogic.Dtos.VoteBooking;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Areas.Admin.Controllers
{
    public class VoteBookingController(IVoteBookingServices voteBookingServices) : AdminBaseController
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
        public async Task<IActionResult> GetAllPaging(VoteBookingRequest request)
        {
            var result = await voteBookingServices.GetPagination(request);
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
            var result = await voteBookingServices.GetById(id);

            return Json(result);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntity(VoteBookingDto request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (request.Id == 0)
            {
                var result = await voteBookingServices.Add(request);
                return Json(result);
            }
            else
            {
                var result = await voteBookingServices.Update(request);
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

            var result = await voteBookingServices.Delete(id);

            return Json(result);
        }
    }
}

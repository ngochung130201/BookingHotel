using BusinessLogic.Dtos.Rooms;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Areas.Admin.Controllers
{
    public class RoomController(IRoomsService roomsService) : AdminBaseController
    {
        public async Task<IActionResult> IndexAsync()
        {
            var result = await roomsService.GetRoomTypesName();
            if (result.Data == null) { return View(new List<RoomTypesResponse>()); }
            return View(result.Data);
        }

        /// <summary>
        /// Get Brand Pagination
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPagination(RoomsRequest request)
        {
            var result = await roomsService.GetPagination(request);
            return Json(result);
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetById(short id)
        {
            var result = await roomsService.GetById(id);

            return Json(result);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntity(RoomsDto request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (request.Id == 0)
            {
                var result = await roomsService.Add(request);
                return Json(result);
            }
            else
            {
                var result = await roomsService.Update(request);
                return Json(result);
            }
        }

        /// <summary>
        /// Delete by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(short id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = await roomsService.Delete(id);

            return Json(result);
        }


        /// <summary>
        /// Change Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await roomsService.ChangeStatusAsync(id);
            return Json(result);
        }
    }
}

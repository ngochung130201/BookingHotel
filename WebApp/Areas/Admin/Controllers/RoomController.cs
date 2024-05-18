using BusinessLogic.Dtos.Rooms;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Areas.Admin.Controllers
{
    public class RoomController(IRoomsService roomsService,
                                IRoomsImageService roomsImageService) : AdminBaseController
    {
        public async Task<IActionResult> IndexAsync()
        {
            var result = await roomsService.GetRoomTypesName();
            if (result.Data == null) { return View(new List<RoomTypesResponse>());  }
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

        /// <summary>
        /// Import data from excel
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportData(IFormFile file)
        {
            var result = await roomsService.ImportData(file);
            if (!result.Succeeded)
            {
                return Json(new { succeeded = false, errors = result.Messages });
            }

            return Json(new { succeeded = true, message = "Import thành công!" });
        }

        /// <summary>
        /// Download file template
        /// </summary>
        /// <returns></returns>        
        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            var content = await roomsService.CreateTemplate();
            if (content == null) return NotFound();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RoomTemplate.xlsx");
        }

        [HttpGet]
        public IActionResult ExportExcel(RoomsRequest request)
        {
            var result = roomsService.ExportExcel(request);
            if (result == null) return NotFound();
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Room.xlsx");
        }

        /// <summary>
        /// Get Rooms Image Pagination
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoomImagePagination(RoomImageRequest request)
        {
            var result = await roomsImageService.GetPagination(request);
            return Json(result);
        }

        /// <summary>
        /// Save Room Image
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveRoomImage(RoomImageDto request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            var result = await roomsImageService.Add(request);
            return Json(result);
        }

        /// <summary>
        /// Delete by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteRoomImage(short id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = await roomsImageService.Delete(id);

            return Json(result);
        }
    }
}

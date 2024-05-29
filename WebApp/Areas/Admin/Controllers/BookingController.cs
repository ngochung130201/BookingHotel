using BusinessLogic.Dtos.Booking;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Admin.Controllers
{
    public class BookingController(IBookingService bookingService) : AdminBaseController
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
        public async Task<IActionResult> GetAllPaging(BookingRequest request)
        {
            var result = await bookingService.GetPagination(request);
            return Json(result);
        }

        [HttpGet]
        public IActionResult ExportExcel(BookingRequest request)
        {
            var result = bookingService.ExportExcel(request);
            if (result == null) return NotFound();
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Bookings.xlsx");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await bookingService.Delete(id);
            return Json(result);
        }
        // ChangeStatus
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await bookingService.ChangeStatusAsync(id);
            return Json(result);
        }
        // SaveEntity
        [HttpPost]
        public async Task<IActionResult> SaveEntity(BookingDto request)
        {
            var result = await bookingService.Update(request);
            return Json(result);
        }
        // GetBookingById
        [HttpGet]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var result = await bookingService.GetById(id);
            return Json(result);
        }
        // Notification
        [HttpGet]
        public async Task<IActionResult> Notification()
        {
            var result = await bookingService.GetNotification();
            return Json(result);
        }
    }
}

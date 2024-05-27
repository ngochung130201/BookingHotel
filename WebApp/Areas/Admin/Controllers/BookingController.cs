using BusinessLogic.Dtos.Booking;
using BusinessLogic.Dtos.Rooms;
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

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetById(int id)
        {
            var result = await bookingService.GetById(id);

            return View(result);
        }

        [HttpGet]
        public IActionResult ExportExcel(BookingRequest request)
        {
            var result = bookingService.ExportExcel(request);
            if (result == null) return NotFound();
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Bookings.xlsx");
        }
    }
}

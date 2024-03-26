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
    }
}

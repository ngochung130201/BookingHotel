using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class RoomController() : Controller
    {
        /// <summary>
        /// Room
        /// </summary>
        /// <returns></returns>
        [Route("room")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
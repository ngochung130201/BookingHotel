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

        /// <summary>
        /// Room details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Route("room-details/{id}")]
        [Route("blog-details")]
        public IActionResult RoomDetails(int id)
        {
            return View();
        }
    }
}
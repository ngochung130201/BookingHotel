using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController() : Controller
    {
        /// <summary>
        /// Home
        /// </summary>
        /// <returns></returns>
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

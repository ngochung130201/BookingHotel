using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AboutUsController() : Controller
    {
        /// <summary>
        /// About Us
        /// </summary>
        /// <returns></returns>
        [Route("about-us")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
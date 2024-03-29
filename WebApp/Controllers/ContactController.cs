using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ContactController() : Controller
    { /// <summary>
      /// Contact
      /// </summary>
      /// <returns></returns>
        [Route("contact")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

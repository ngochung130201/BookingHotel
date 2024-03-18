using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Admin.Controllers
{
    public class Error404Controller() : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

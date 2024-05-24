using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class PaymentController : Controller
    {
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            return View();
        }
    }
}

using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Admin.Controllers
{
    public class DashboardController(IDashboardService dashboardService) : AdminBaseController
    {
        public async Task<IActionResult> Index()
        {
            var result = await dashboardService.GetDashboard();
            return View(result);
        }
    }
}

using BusinessLogic.Dtos.News;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace WebApp.Controllers
{
    public class BlogController() : Controller
    {   /// <summary>
        /// Blog
        /// </summary>
        /// <returns></returns>
        [Route("/blog")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Blog details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Route("blog-details/{id}")]
        [Route("/blog/blog-details")]
        public IActionResult BlogDetails(int id)
        {
            return View();
        }
    }
}

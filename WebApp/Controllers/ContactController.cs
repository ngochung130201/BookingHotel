using BusinessLogic.Dtos.FeedBacks;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Controllers
{
    public class ContactController(IFeedBackService feedBackService) : Controller
    { /// <summary>
      /// Contact
      /// </summary>
      /// <returns></returns>
        [Route("contact")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Save entity Feedback
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("PostFeedback")]
        public async Task<IActionResult> PostFeedback(FeedBacksDto request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
                
            var result = await feedBackService.Add(request);

            return Json(result);
        }
    }
}

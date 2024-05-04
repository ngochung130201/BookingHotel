using BusinessLogic.Dtos.Comment;
using BusinessLogic.Dtos.News;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Controllers
{
    public class BlogController(INewsService newsService,
                                ICommentService commentService,
                                IReplyCommentService replyCommentService) : Controller
    {   
        /// <summary>
        /// Blogs
        /// </summary>
        /// <returns></returns>
        [Route("/blog")]
        public async Task<IActionResult> Index()
        {
            var result = await newsService.GetPagination(new NewRequest
            {
                PageNumber = 1,
                PageSize = 5,
                Status = true,
            });

            if (!result.Data.Any()) result.Data = new List<NewsResponse>();

            return View(result);
        }

        [HttpGet]
        [Route("blog/get-data-paging/{page}")]
        public async Task<IActionResult> GetDataPaging(int page)
        {
            var newsRequest = new NewRequest
            {
                PageNumber = page,
                PageSize = 5,
                Status = true,
            };

            var resultNews = await newsService.GetPagination(newsRequest);

            // Tạo danh sách các NewsResponse
            var newsResponses = new List<NewsResponse>();

            foreach (var news in resultNews.Data)
            {
                // Tạo một NewsResponse mới cho mỗi tin tức
                var newsResponse = new NewsResponse
                {
                    Id = news.Id,
                    Title = news.Title,
                    Thumbnail = news.Thumbnail,
                    Content = news.Content,
                    Status = news.Status,
                    Hot = news.Hot,
                    CreatedOn = news.CreatedOn,
                    CreatedBy = news.CreatedBy
                };

                // Lấy danh sách bình luận cho mỗi tin tức
                var commentRequest = new CommentRequest
                {
                    NewId = news.Id
                };
                var resultComment = await commentService.GetPagination(commentRequest);
                newsResponse.Comments = resultComment.Data;

                // Thêm NewsResponse vào danh sách
                newsResponses.Add(newsResponse);
            }

            // Trả về PartialView với danh sách NewsResponse
            return PartialView("_BlogPartial", newsResponses);
        }


        /// <summary>
        /// Blog details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("blog-details/{id}")]
        public async Task<IActionResult> BlogDetails(int id)
        {  
            var result = new ClientNewsDetailsResponse();
            var resultNewsDetails = await newsService.GetById(id);
            var resultComment = await commentService.GetPagination(new CommentRequest
            {
                NewId = id,
            });

            result.Comments = resultComment.Data;
            result.NewsDetail = resultNewsDetails.Data;
            return View(result);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntity(CommentDto request)
        {
            string currentUrl = HttpContext.Request.Path;

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (request.Id == 0)
            {
                var result = await commentService.Add(request);
                return Json(result);
            }
            else
            {
                var result = await commentService.Update(request);
                return Json(result);
            }
        } 
    }
}

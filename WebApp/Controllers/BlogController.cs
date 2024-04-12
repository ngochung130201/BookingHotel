using BusinessLogic.Dtos.Comment;
using BusinessLogic.Dtos.News;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class BlogController(INewsService newsService,
                                ICommentService commentService,
                                IReplyCommentService replyCommentService) : Controller
    {   /// <summary>
        /// Blog
        /// </summary>
        /// <returns></returns>
        [Route("/blog")]
        public async Task<IActionResult> Index()
        {
            var result = new ClientNewsResponse();
            var resultNews = await newsService.GetPagination(new NewRequest{
                PageNumber = 1,
                PageSize = 5
            });

            result.News = resultNews.Data;

            return View(result);
        }

        /// <summary>
        /// Blog details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/blog/blog-details/{id}")]
        public async Task<IActionResult> BlogDetails(int id)
        {
            var result = new ClientNewsDetailResponse();
            var resultNewsDetail = await newsService.GetById(id);
            var resultComment = await commentService.GetPagination(new CommentRequest
            {
                NewId = id,
            });
            foreach (var comment in resultComment.Data)
            {
                var resultReplyComment = await replyCommentService.GetPagination(new ReplyCommentRequest
                {
                    CommentId = comment.Id,
                });
                result.Replies = resultReplyComment.Data;
            }

            result.Comments = resultComment.Data;
            result.NewsDetail = resultNewsDetail.Data;

            return View(result);
        }
    }
}

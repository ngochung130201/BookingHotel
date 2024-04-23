using BusinessLogic.Dtos.Comment;
using BusinessLogic.Dtos.Rooms;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Controllers
{
    public class RoomController(IRoomTypesService roomTypesService,
                                IRoomsService roomsService,
                                ICommentService commentService,
                                IReplyCommentService replyCommentService) : Controller
    {
        /// <summary>
        /// Room
        /// </summary>
        /// <returns></returns>
        [Route("room")]
        public async Task<IActionResult> Index()
        {
            var result = new ClientRoomsResponse();
            var resultRoomTypes = await roomTypesService.GetPagination(new RoomTypesRequest
            {
                PageNumber = 1,
                PageSize = 4
            });

            var resultRooms = await roomsService.GetPagination(new RoomsRequest
            {
                PageNumber = 1,
                PageSize = 20,
            });

            result.Rooms = resultRooms.Data;
            result.RoomTypes = resultRoomTypes.Data;
            return View(result);
        }

        /// <summary>
        /// Room details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("room-details/{id}")]
        public async Task<IActionResult> RoomDetails(int id)
        {
            var result = new ClientRoomDetailResponse();
            var resultRoomDetail = await roomsService.GetById(id);
            var resultComment = await commentService.GetPagination(new CommentRequest
            {
                RoomId = id,
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
            result.Room = resultRoomDetail.Data;

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
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (request.Id == 0)
            {
                var result = await commentService.Add(request);
                return Redirect($"/room-details/{request.RoomId}");
            }
            else
            {
                var result = await commentService.Update(request);
                return Redirect($"/room-details/{request.RoomId}");
            }
        }

        /// <summary>
        /// Save entity reply
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntityReply(ReplyCommentDto request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (request.Id == 0)
            {
                var result = await replyCommentService.Add(request);
            }
            else
            {
                var result = await replyCommentService.Update(request);
            }
            string currentUrl = HttpContext.Request.Path;

            return Redirect(currentUrl);
        }
    }
}
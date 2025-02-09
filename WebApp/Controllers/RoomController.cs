﻿using BusinessLogic.Dtos.Booking;
using BusinessLogic.Dtos.Comment;
using BusinessLogic.Dtos.Rooms;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Dtos.Service;
using BusinessLogic.Services;
using BusinessLogic.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Controllers
{
    public class RoomController(IRoomTypesService roomTypesService,
                                IRoomsService roomsService,
                                ICommentService commentService,
                                IReplyCommentService replyCommentService,
                                IBookingService bookingService,
                                ICurrentUserService currentUserService,
                                IServicesServices servicesServices) : Controller
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
            var fullName = currentUserService.FullName;
            var email = currentUserService.Email;
            var phoneNumber = currentUserService.PhoneNumber;
            var userId = currentUserService.UserId;

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

            var resultServices = await servicesServices.GetPagination(new ServiceRequest
            {
                PageNumber = 1,
                PageSize = 12
            });

            result.Services = resultServices.Data;
            result.Comments = resultComment.Data;
            result.Room = resultRoomDetail.Data;
            result.Email = email;
            result.PhoneNumber = phoneNumber;
            result.FullName = fullName;
            result.UserId = userId;

            return View(result);
        }

        /// <summary>
        /// Save entity booking
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveBooking(BookingDto request)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (request.Id == 0)
            {
                var result = await bookingService.Add(request);
                return Json(result);
            }
            else
            {
                var result = await bookingService.Update(request);
                return Json(result);
            }
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
                return Json(result);
            }
            else
            {
                var result = await commentService.Update(request);
                return Json(result);
            }
        }

        /// <summary>
        /// Get list room
        /// </summary>
        /// <param name="RoomTypesId"></param>
        /// <returns></returns>
        [Route("list-room/{RoomTypesId}")]
        public async Task<IActionResult> ListRoom(int RoomTypesId)
        {
            var result = await roomsService.GetPagination(new RoomsRequest
            {
                PageNumber = 1,
                PageSize = 20,
                RoomTypes = RoomTypesId,
            });

            return View(result);
        }
    }
}
using BusinessLogic.Dtos.Home;
using BusinessLogic.Dtos.News;
using BusinessLogic.Dtos.Rooms;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class RoomController(IRoomTypesService roomTypesService,
                                 IRoomsService roomsService) : Controller
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
                PageSize = 8,
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
        //[Route("room-details/{id}")]
        [Route("blog-details")]
        public IActionResult RoomDetails(int id)
        {
            return View();
        }
    }
}
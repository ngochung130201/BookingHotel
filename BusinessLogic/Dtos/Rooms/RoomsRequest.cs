using BusinessLogic.Request;

namespace BusinessLogic.Dtos.Rooms
{
    public class RoomsRequest : RequestParameter
    {
        public int? RoomTypes { get; set; }
        public bool? Status { get; set; }
    }
}

using BusinessLogic.Request;

namespace BusinessLogic.Dtos.Booking
{
    public class BookingRequest : RequestParameter
    {
        public short? Status { get; set; }
        public string? UserId { get; set; }
    }
}

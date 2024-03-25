using BusinessLogic.Request;

namespace BusinessLogic.Dtos.Booking
{
    public class BookingRequest : RequestParameter
    {
        public bool? Status { get; set; }
    }
}

namespace BusinessLogic.Dtos.VoteBooking
{
    public class VoteBookingResponse
    {
        public int Id { get; set; } = default!;

        public string? BookingCode { get; set; }

        public string? FullName { get; set; }

        public short Star { get; set; } 

        public string? Title { get; set; } 

        public string? Comment { get; set; } 

        public bool? Status { get; set; }

        public int BookingId { get; set; } 

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}

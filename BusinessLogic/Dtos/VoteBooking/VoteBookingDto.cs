namespace BusinessLogic.Dtos.VoteBooking
{
    public class VoteBookingDto
    {
        public int Id { get; set; } = default!;

        public short Star { get; set; }

        public string? Title { get; set; }

        public string? Comment { get; set; }

        public bool? Status { get; set; }

        public int BookingId { get; set; }
    }
}

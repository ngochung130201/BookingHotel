namespace BusinessLogic.Dtos.Booking
{
    public class BookingResponse
    {
        public int Id { get; set; } = default!;

        public string BookingCode { get; set; } = default!;

        public DateTime? TransactionDate { get; set; }

        public DateTime? CheckInDate { get; set; }

        public short? Status { get; set; }

        public decimal TotalAmount { get; set; }

        public string UserId { get; set; } = default!;

        public string? FullName { get; set; }

        public int BookedRoomNumber { get; set; }

        public int ServicesArising { get; set;}

        public DateTime? CreatedOn { get; set; }
    }
}

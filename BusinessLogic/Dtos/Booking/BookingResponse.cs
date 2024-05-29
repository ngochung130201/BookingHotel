namespace BusinessLogic.Dtos.Booking
{
    public class BookingResponse
    {
        public int Id { get; set; } = default!;

        public string BookingCode { get; set; } = default!;

        public DateTime? TransactionDate { get; set; }

        public DateTime? CheckInDate { get; set; }

        public DateTime? CheckOutDate { get; set; }

        public short? Status { get; set; }

        public decimal TotalAmount { get; set; }

        public string UserId { get; set; } = default!;

        public string? FullName { get; set; }

        public int BookedRoomNumber { get; set; }

        public List<ServiceData>? ServicesArising { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
    public class ServiceData
    {
        public int BookingId { get; set; }
        public string Name { get; set; }
    }
}

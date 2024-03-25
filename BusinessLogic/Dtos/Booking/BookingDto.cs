namespace BusinessLogic.Dtos.Booking
{
    public class BookingDto
    {
        public int Id { get; set; } = default!;

        public string BookingCode { get; set; } = default!; 

        public DateTime? TransactionDate { get; set; } 

        public DateTime? CheckInDate { get; set; }

        public DateTime? CheckOutDate { get; set; }

        public bool? Status { get; set; } 

        public short? Adult { get; set; }

        public short? Kid { get; set; } 

        public decimal TotalAmount { get; set; } 

        public string? Payment { get; set; } 

        public string? Message { get; set; } 

        public string UserId { get; set; } = default!; 
    }
}

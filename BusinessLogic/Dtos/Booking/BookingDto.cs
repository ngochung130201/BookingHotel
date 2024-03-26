namespace BusinessLogic.Dtos.Booking
{
    public class BookingDto
    {
        public int Id { get; set; } = default!;

        public string? BookingCode { get; set; } = default!; 

        public DateTime? TransactionDate { get; set; } 

        public DateTime? CheckInDate { get; set; }

        public DateTime? CheckOutDate { get; set; }

        public short? Status { get; set; } 

        public short? Adult { get; set; }

        public short? Kid { get; set; } 

        public decimal? TotalAmount { get; set; } 

        public string? Payment { get; set; } 

        public string? Message { get; set; } 

        public string? UserId { get; set; } 

        public string? FullName { get; set; }

        public int[]? RoomId { get; set; }

        public decimal? DownPayment { get; set; }

        public List<BookingDetailDto>? BookingDetailDto { get; set; }

        public List<CostBookingDto>? CostBookingDto { get; set; }

        public SpecialDayBookingDto? SpecialDayBookingDto { get; set; }
    }

    public class CostBookingDto
    {
        public int? CostId { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; } 
    }

    public class BookingDetailDto
    {
        public int? RoomId { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }
    }

    public class SpecialDayBookingDto
    {
        public int? Id { get; set; }

        public string? Title { get; set; } 

        public int? PercentDiscount { get; set; } 

        public string? Description { get; set; } 
    }
}

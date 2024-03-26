namespace BusinessLogic.Enums
{
    public enum StatusBooking : short
    {
        NewBooking = 1, // Đơn mới
        DownPayment = 2, // Đã đặt cọc
        CheckIn = 3, // Đã nhận phòng
        CheckOut = 4 // Đã trả phòng
    }
}

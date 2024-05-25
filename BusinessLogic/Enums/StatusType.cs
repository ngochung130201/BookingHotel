namespace BusinessLogic.Enums
{
    public enum StatusBooking : short
    {
        NewBooking = 1, // Đơn mới
        DownPayment = 2, // Đã đặt cọc
        Payment = 3, // Đã thanh toán
        CheckIn = 4, // Đã nhận phòng
        CheckOut = 5 // Đã trả phòng
    }
}

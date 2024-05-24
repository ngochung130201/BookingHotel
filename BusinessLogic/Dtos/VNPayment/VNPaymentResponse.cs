namespace BusinessLogic.Dtos.VNPayment
{
    public class VNPaymentResponse
    {
        public bool Success { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderDescription { get; set; }
        public string? OrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public string? Token { get; set; }
        public string? VnPayResponseCode { get; set; }
    }

    public class VNPaymentRequest
    {
        public string? FullName { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

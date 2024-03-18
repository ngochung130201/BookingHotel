namespace BusinessLogic.Dtos.Invoice
{
    public class InvoiceResponseDto
    {
        public string ErrorCode { get; set; } = default!;
        public string Description { get; set; } = default!;
        public ResultDto Result { get; set; } = new();

        public class ResultDto
        {
            public string SupplierTaxCode { get; set; } = default!;
            public string InvoiceNo { get; set; } = default!;
            public string TransactionId { get; set; } = default!;
            public string ReservationCode { get; set; } = default!;
            public string CodeOfTax { get; set; } = default!;
        }
    }
}

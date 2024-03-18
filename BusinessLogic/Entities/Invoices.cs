using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class Invoices : AuditableBaseEntity<long>
    {
        public int TicketId { get; set; }
        public string InvoicesNo { get; set; } = default!;
        public string? TransactionUuid { get; set; }
        [StringLength(255)]
        public string InvoiceType { get; set; } = default!;
        [StringLength(255)]
        public string TemplateCode { get; set; } = default!;
        [StringLength(255)]
        public string CurrencyCode { get; set; } = default!;
        [StringLength(255)]
        public string AdjustmentType { get; set; } = default!;
        public bool PaymentStatus { get; set; }
        [StringLength(255)]
        public string UserName { get; set; } = default!;
        [StringLength(255)]
        public string BuyerName { get; set; } = default!;
        [StringLength(255)]
        public string? BuyerTaxCode { get; set; }
        [StringLength(255)]
        public string BuyerAddressLine { get; set; } = default!;
        [StringLength(255)]
        public string BuyerPhoneNumber { get; set; } = default!;
        [StringLength(255)]
        public string BuyerIdNo { get; set; } = default!;
        [StringLength(255)]
        public string BuyerIdType { get; set; } = default!;
        [StringLength(255)]
        public string PaymentMethodName { get; set; } = default!;
        [StringLength(255)]
        public string ItemCode { get; set; } = default!;
        [StringLength(255)]
        public string ItemName { get; set; } = default!;
        [StringLength(255)]
        public string UnitName { get; set; } = default!;
        public decimal UnitPrice { get; set; } = default!;
        public decimal Quantity { get; set; }
        public long ItemTotalAmountWithoutTax { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal Discount { get; set; }
        public decimal ItemDiscount { get; set; }
        [StringLength(255)]
        public string TotalAmountWithTaxInWords { get; set; } = default!;
        [StringLength(255)]
        public long TotalAmountWithTax { get; set; } = default!;
        public string? ReservationCode { get; set; }
        public string? CodeOfTax { get; set; }
    }
}

using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class PriceManager : AuditableBaseEntity<int>
    {
        public DateTime Date { get; set; } // Ngày đặc biệt (ngày lễ)

        public int? PercentDiscount { get; set; } // Phần trăm giảm giá

        public int? PercentIncrease { get; set; } // Phần trăm tăng giá

        public string? Description { get; set; } // Mô tả
    }
}

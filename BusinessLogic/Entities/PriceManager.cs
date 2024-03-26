using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class PriceManager : AuditableBaseEntity<int>
    {
        public DateTime Date { get; set; } // Ngày đặc biệt (ngày lễ)

        public string? Title { get; set; } // Tiêu đề

        public int? PercentDiscount { get; set; } // Phần trăm giảm giá

        public string? Description { get; set; } // Mô tả
    }
}

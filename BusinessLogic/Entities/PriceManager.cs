using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class PriceManager : AuditableBaseEntity<int>
    {
        public DateTime SinceDay { get; set; } // Ngày từ ngày

        public DateTime ToDay { get; set; } // Ngày đến ngày

        public string? Title { get; set; } // Tiêu đề

        public int? PercentDiscount { get; set; } // Phần trăm giảm giá

        public string? Description { get; set; } // Mô tả
    }
}

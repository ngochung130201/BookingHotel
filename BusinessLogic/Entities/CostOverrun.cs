using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class CostOverrun : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(255)]
        public string? Name { get; set; } // Tên chi phí phát sinh

        public string? Description { get; set; } // Mô tả chi phí phát sinh

        public decimal Price { get; set; } // Giá tiền của chi phí phát sinh

        [StringLength(255)]
        public string? Image { get; set; } // Ảnh của chi phí phát sinh
    }
}

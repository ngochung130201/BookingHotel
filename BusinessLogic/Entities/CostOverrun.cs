using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class CostOverrun : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(255)]
        public string? Name { get; set; } // Tên dịch vụ phát sinh

        public string? Description { get; set; } // Mô tả dịch vụ phát sinh

        public decimal Price { get; set; } // Giá tiền của dịch vụ phát sinh

        [StringLength(255)]
        public string? Image { get; set; } // Ảnh của dịch vụ phát sinh
    }
}

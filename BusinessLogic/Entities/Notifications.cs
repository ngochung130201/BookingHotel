using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class Notifications : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string? Type { get; set; } // Kiểu thao thác thực hiện

        [StringLength(255)]
        public string? NotifiableType { get; set; } // Models thực hiện việc thay đổ

        [Required]
        [StringLength(255)]
        public string? UserId { get; set; } // Người thực hiện

        public string? Data { get; set; } // Dữ liệu truyền lên

        public DateTime? ReadAt { get; set; } // Thời gian xem thông báo
    }
}

using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class Audits : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string? UserType { get; set; } // Tên model user thực hiện thay đổi

        [Required]
        [StringLength(255)]
        public string UserId { get; set; } = default!; // Id user thực hiện thay đổi

        [StringLength(255)]
        public string? Event { get; set; } // Hành động

        [StringLength(255)]
        public string? AuditableType { get; set; } // Tên model bị thay đổi

        public int? AuditableId { get; set; } // Id bị thay đổi

        [StringLength(255)]
        public string? OldValues { get; set; } // Giá trị cũ

        [StringLength(255)]
        public string? NewValues { get; set; } // Giá trị mới

        public string? url { get; set; } // Link xử lý công việc

        [StringLength(50)]
        public string? IpAddress { get; set; } // Địa chỉ ip

        [StringLength(255)]
        public string? UserAgent { get; set; } // Thông tin vật lý của thiết bị

        [StringLength(255)]
        public string? Tags { get; set; } // Nhãn
    }
}

using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class VoteBooking : AuditableBaseEntity<int>
    {
        public short Star { get; set; } // Số sao

        [StringLength(255)]
        public string? Title { get; set; } // Tiêu đề

        [StringLength(255)]
        public string? Comment { get; set; } // Nội dung comment

        public bool? Status { get; set; } // Trạng thái

        [Required]
        public int BookingId { get; set; } // Đơn đặt phòng
    }
}

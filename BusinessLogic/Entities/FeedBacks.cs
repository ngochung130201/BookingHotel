using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class FeedBacks : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(255)]
        public string? Name { get; set; } // Tên người viết phản hồi

        [Required]
        [StringLength(255)]
        public string? Email { get; set; } // Email người phản hồi

        [StringLength(255)]
        public string? Title { get; set; } // Tiêu đề phản hồi

        public string? Content { get; set; } // Nội dung phản hồi

        public string? Reply { get; set; } // Nội dung trả lời phản hồi

        [StringLength(255)]
        public string? ReplyBy { get; set; } // Người phản hồi
    }
}

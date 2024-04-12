using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class News : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string? Title { get; set; } // Tiêu đề bài viết

        [StringLength(255)]
        public string? Thumbnail { get; set; } // Ảnh đại diện

        public string? Content { get; set; } // Nội dung bài viết

        public string? Description { get; set; }

        public bool? Status { get; set; } // Trạng thái bài viết

        public bool? Hot { get; set; } // Bài viết nổi bật
    }
}

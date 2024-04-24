using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class News : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string? Title { get; set; } // Tiêu đề bài viết

        [StringLength(255)]
        public string? NewsType { get; set; } // Thể loại bài viết

        [StringLength(55)]
        public string? Author { get; set; } // Tác giả bài viết
        
        [StringLength(255)]
        public string? Thumbnail { get; set; } // Ảnh đại diện

        public string? Content { get; set; } // Nội dung bài viết

        public string? Summary { get; set; } // Tóm tắt bài viết

        public bool? Status { get; set; } // Trạng thái bài viết

        public bool? Hot { get; set; } // Bài viết nổi bật
    }
}

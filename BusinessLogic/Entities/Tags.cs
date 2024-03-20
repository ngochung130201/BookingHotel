using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class Tags : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(255)]
        public string TagName { get; set; } = default!;

        [StringLength(255)]
        public string? Slug { get; set; }
    }
}

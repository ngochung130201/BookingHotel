using System.ComponentModel.DataAnnotations;
using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class Function : AuditableBaseEntity<int>
    {
        [StringLength(128)]
        public string FunctionId { get; set; } = default!;

        [Required]
        [StringLength(128)]
        public string Name { set; get; } = default!;

        [StringLength(250)]
        public string? URL { set; get; }

        [StringLength(128)]
        public string? ParentId { set; get; }

        public string? IconCss { get; set; }
        public int SortOrder { set; get; }
    }
}
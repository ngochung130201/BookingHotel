using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class Partners : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string Title { get; set; } = default!;

        [StringLength(255)]
        public string TitleVi { get; set; } = default!;

        [DataType("text")]
        public string? Description { get; set; }

        [DataType("text")]
        public string? DescriptionVi { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }
        
        public short Status { set; get; }
    }
}

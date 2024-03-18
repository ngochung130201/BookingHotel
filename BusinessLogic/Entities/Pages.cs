using System.ComponentModel.DataAnnotations;
using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class Pages : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string Title { get; set; } = default!;

        [StringLength(255)]
        public string TitleVi { get; set; } = default!;

        [DataType("text")]
        public string? Content { get; set; }

        [DataType("text")]
        public string? ContentVi { get; set; }

        [MaxLength(256)]
        public string Alias { set; get; } = default!;

        public short Status { set; get; }
    }
}
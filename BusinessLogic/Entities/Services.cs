using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class Services : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [StringLength(255)]
        public string? Image {  get; set; }
    }
}

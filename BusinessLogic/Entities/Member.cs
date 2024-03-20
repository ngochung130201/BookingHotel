using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class Member : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(255)]
        public string Email { get; set; } = default!;
    }
}

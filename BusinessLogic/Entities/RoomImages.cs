using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class RoomImages : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string? UrlImage { get; set; }

        [Required]
        public int RoomId { get; set; }
    }
}

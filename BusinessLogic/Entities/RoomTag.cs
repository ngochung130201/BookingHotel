using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class RoomTag : AuditableBaseEntity<int>
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public int TagId { get; set; }
    }
}

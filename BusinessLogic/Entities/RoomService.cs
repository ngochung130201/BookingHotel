using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class RoomService : AuditableBaseEntity<int>
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public int ServiceId { get; set; }
    }
}

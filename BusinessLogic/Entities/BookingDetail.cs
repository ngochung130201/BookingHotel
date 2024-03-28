using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class BookingDetail : AuditableBaseEntity<int>
    {
        [Required]
        public int RoomId { get; set; } // Phòng

        [Required]
        public int BookingId { get; set; } // Đơn đặt phòng
    }
}

using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    internal class SpecialDayBooking : AuditableBaseEntity<int>
    {
        [Required]
        public int BookingId { get; set; } // Đơn đợt phòng

        [Required]
        public int SpecialDayId { get; set; } // Ngày đặc biệt (PriceManager)
    }
}

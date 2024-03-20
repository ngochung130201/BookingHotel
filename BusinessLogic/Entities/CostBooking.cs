using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class CostBooking : AuditableBaseEntity<int>
    {
        [Required]
        public int BookingId { get; set; } // Đơn đặt phòng

        [Required]
        public int CostId { get; set; } // Dịch vụ phát sinh

        [Required]
        public decimal Price { get; set; } // Giá tiền dịch vụ phát sinh
    }
}

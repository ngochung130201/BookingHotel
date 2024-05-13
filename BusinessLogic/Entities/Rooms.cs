using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class Rooms : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [StringLength(255)]
        public string? Thumbnail { get; set; }

        [StringLength(255)]
        public string? RoomCode { get; set; }

        public int RoomTypeId { get; set; }

        public decimal Price { get; set; }

        [StringLength(255)]
        public string? Location { get; set; }

        public short? Adult { get; set; }

        public short? Kid { get; set; }

        [StringLength(255)]
        public string? Acreage { get; set; }

        public bool Status { get; set; }

        [StringLength(255)]
        public string? Views { get; set; }

        public short? StatusRoom { get; set; }
    }
}

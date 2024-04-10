using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class Comment : AuditableBaseEntity<int>
    {
        public string? Content { get; set; } 

        public string? UserId { get; set; }

        public int? RoomId { get; set; }

        public int? NewId { get; set; }
    }
}

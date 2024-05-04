using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class ReplyComment : AuditableBaseEntity<int>
    {
        public int? CommentId { get; set; }

        public string? Content { get; set; } 

        public string? UserId { get; set; }
    }
}

namespace BusinessLogic.Dtos.Comment
{
    public class ReplyCommentResponse
    {    
        public int Id { get; set; } = default!;

        public int? CommentId { get; set; }

        public string? Content { get; set; }

        public string? UserId { get; set; }

        public string? FullName { get; set; }

        public string? Avatar { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}

namespace BusinessLogic.Dtos.Comment
{
    public class ReplyCommentDto
    {
        public int Id { get; set; } = default!;

        public int CommentId { get; set; }

        public string? Content { get; set; }

        public string? UserId { get; set; }
    }
}

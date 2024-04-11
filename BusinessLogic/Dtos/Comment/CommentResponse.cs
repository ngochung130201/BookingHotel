namespace BusinessLogic.Dtos.Comment
{
    public class CommentResponse
    {
        public int Id { get; set; } = default!;

        public string? Content { get; set; }

        public string? FullName { get; set; }

        public string? Avatar { get; set; }

        public string? UserId { get; set; }

        public int? RoomId { get; set; }

        public int? NewId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}

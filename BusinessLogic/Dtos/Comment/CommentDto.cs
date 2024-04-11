namespace BusinessLogic.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; } = default!;

        public string? Content { get; set; }

        public string? UserId { get; set; }

        public int? RoomId { get; set; }

        public int? NewId { get; set; }

        public string? FullName { get; set; }

        public string? Avatar { get; set; }
    }
}

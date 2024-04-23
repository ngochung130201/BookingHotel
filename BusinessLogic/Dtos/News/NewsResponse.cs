using BusinessLogic.Dtos.Comment;

namespace BusinessLogic.Dtos.News
{
    public class NewsResponse
    {
        public int Id { get; set; } = default!;

        public string? Title { get; set; }

        public string? Thumbnail { get; set; }

        public string? Content { get; set; }

        public string? NewsType { get; set; }

        public string? Author { get; set; }

        public bool? Status { get; set; }

        public bool? Hot { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;

        public List<CommentResponse> Comments { get; set; } = new();
        public List<ReplyCommentResponse> Replies { get; set; } = new();
    }
}

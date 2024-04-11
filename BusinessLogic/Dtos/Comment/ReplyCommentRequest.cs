using BusinessLogic.Request;

namespace BusinessLogic.Dtos.Comment
{
    public class ReplyCommentRequest : RequestParameter
    {
        public int? CommentId { get; set; }
    }
}

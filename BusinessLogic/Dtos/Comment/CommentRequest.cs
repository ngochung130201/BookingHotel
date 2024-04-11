using BusinessLogic.Request;

namespace BusinessLogic.Dtos.Comment
{
    public class CommentRequest : RequestParameter
    {
        public int? RoomId { get; set; }

        public int? NewId { get; set; }
    }
}

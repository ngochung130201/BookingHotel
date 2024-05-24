using BusinessLogic.Dtos.Comment;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Dtos.Service;
using BusinessLogic.Services;

namespace BusinessLogic.Dtos.Rooms
{
    public class RoomsResponse
    {
        public int Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? Thumbnail { get; set; }

        public string? RoomCode { get; set; }

        public int RoomTypeId { get; set; }

        public string? RoomTypeName { get; set; }

        public decimal Price { get; set; }

        public string? Location { get; set; }

        public short? Adult { get; set; }

        public short? Kid { get; set; }

        public string? Acreage { get; set; }

        public bool Status { get; set; }

        public string? Views { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;
    }

    public class ClientRoomsResponse
    {
        public List<RoomsResponse> Rooms { get; set; } = new();
        public List<RoomTypesResponse> RoomTypes { get; set; } = new();
    }

    public class ClientRoomDetailResponse
    {
        public RoomsDto Room { get; set; } = new();
        public List<CommentResponse> Comments { get; set; } = new();
        public List<ReplyCommentResponse> Replies { get; set; } = new();
        public List<ServiceResponse> Services { get; set; } = new();
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? UserId { get; set; }
    }
}

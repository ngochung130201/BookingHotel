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
}

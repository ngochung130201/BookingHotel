namespace BusinessLogic.Dtos.RoomTypes
{
    public class RoomTypesResponse
    {
        public int Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public short? Star { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}
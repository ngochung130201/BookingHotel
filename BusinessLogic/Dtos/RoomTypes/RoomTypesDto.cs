namespace BusinessLogic.Dtos.RoomTypes
{
    public class RoomTypesDto
    {
        public int Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public short? Star { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }
    }
}

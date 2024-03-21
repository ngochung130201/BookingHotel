namespace BusinessLogic.Dtos.Service
{
    public class ServiceDto
    {
        public int Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? Image { get; set; }
    }
}

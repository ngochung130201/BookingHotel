namespace BusinessLogic.Dtos.Service
{
    public class ServiceResponse
    {
        public int Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? Image { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}

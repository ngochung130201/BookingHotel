namespace BusinessLogic.Dtos.Role
{
    public class RoleResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = default!;
    }
}

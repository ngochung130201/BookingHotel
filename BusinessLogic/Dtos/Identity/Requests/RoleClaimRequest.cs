namespace BusinessLogic.Dtos.Identity.Requests
{
    public class RoleClaimRequest
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = default!;
        public string? Type { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
        public string? Group { get; set; }
        public bool Selected { get; set; }
    }
}
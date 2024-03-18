using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Dtos.Identity.Requests
{
    public class RoleRequest
    {
        public string Id { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }
    }
}
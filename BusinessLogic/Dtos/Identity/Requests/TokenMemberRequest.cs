using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Dtos.Identity.Requests
{
    public class TokenMemberRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        public bool RememberMe { get; set; }
    }
}
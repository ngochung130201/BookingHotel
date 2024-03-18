using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Dtos.Identity.Requests
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}
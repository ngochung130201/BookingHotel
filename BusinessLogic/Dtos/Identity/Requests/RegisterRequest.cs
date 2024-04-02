using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Dtos.Identity.Requests
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConfirmPasswordAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The password and confirmation password do not match.";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (RegisterRequest)validationContext.ObjectInstance;

            if (model.Password != model.ConfirmPassword)
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

    [ConfirmPassword]
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; } = default!;

        public string? PhoneNumber { get; set; }

        [Required]
        public string FirstName { get; set; } = default!;

        [Required]
        public string LastName { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        [Required]
        public string ConfirmPassword { get; set; } = default!;
    }
}
using BusinessLogic.Enums;

namespace BusinessLogic.Dtos.User
{
    public class UserDetailDto
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public string? RoleName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string Roles { get; set; } = default!;
        public string? Address { get; set; }
    }
}

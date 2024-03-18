using BusinessLogic.Enums;

namespace BusinessLogic.Dtos.User
{
    public class UserResponse
    {
        public string Id { get; set; } = default!;
        public string? Email { get; set; }
        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? RoleName { get; set; }
        public DateTime CreatedOn { get; set; }
        public MemberStatus MemberStatus { get; set; }
        public string? UserName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

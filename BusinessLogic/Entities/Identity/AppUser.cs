using BusinessLogic.Entities.Base;
using BusinessLogic.Enums;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Entities.Identity
{
	public class AppUser : IdentityUser<string>, IAuditableEntity<string>
	{
        public override string? Email { get; set; }
        public override string? PhoneNumber { get; set; }
        public string FullName { get; set; } = default!;
		public string? AvatarUrl { get; set; }
		public bool IsActive { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiryTime { get; set; }
		public string CreatedBy { get; set; } = default!;
		public DateTime CreatedOn { get; set; }
		public string? LastModifiedBy { get; set; }
		public DateTime? LastModifiedOn { get; set; }
		public bool IsDeleted { get; set; }
        public string? Password { get; set; }
        public string? CompanyName { get; set; }
        public MemberStatus MemberStatus { get; set; }
		public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
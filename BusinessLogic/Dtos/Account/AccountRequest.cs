namespace BusinessLogic.Dtos.Account
{
    public class AccountRequest
    {
        public string Id { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PasswordCurrent { get; set; }
        public string? PasswordNew { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
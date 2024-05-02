namespace BusinessLogic.Dtos.Home
{
    public class MyAccountResponse
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? FullName { get; set; }
        public  string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}

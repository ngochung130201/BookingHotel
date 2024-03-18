namespace BusinessLogic.Dtos.User
{
    public class ChangePasswordUser
    {
        public string Id { get; set; } = default!;
        public string PasswordNew { get; set; } = default!;
        public string ConfirmPasswordNew { get; set; } = default!;
    }
}
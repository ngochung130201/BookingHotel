using BusinessLogic.Request;

namespace BusinessLogic.Dtos.User
{
    public class UserRequest : RequestParameter
    {
        public string? RoleName { get; set; }
    }
}
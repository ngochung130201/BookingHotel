using BusinessLogic.Request;

namespace BusinessLogic.Dtos.Role
{
    public class RoleRequest : RequestParameter
    {
        public string? RoleName { get; set; }
    }
}
namespace BusinessLogic.Dtos.Role
{
    public class FunctionWithRoleDto
    {
        public string RoleId { get; set; } = default!;

        public string FunctionId { get; set; } = default!;

        public bool CanCreate { set; get; }

        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }

        public bool CanDelete { set; get; }
    }
}
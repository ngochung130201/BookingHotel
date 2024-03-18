using System.ComponentModel.DataAnnotations;
using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class Permission : AuditableBaseEntity<int>
    {
        [Required]
        public string RoleId { get; set; } = default!;

        [Required]
        public string FunctionId { get; set; } = default!;

        public bool CanCreate { set; get; }
        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }
        public bool CanDelete { set; get; }

    }
}
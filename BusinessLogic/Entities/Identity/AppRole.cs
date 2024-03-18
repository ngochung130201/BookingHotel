using BusinessLogic.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Entities.Identity
{
    public class AppRole : IdentityRole, IAuditableEntity<string>
    {
        public string? Description { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<AppRoleClaim> RoleClaims { get; set; }

        public AppRole() : base()
        {
            RoleClaims = new HashSet<AppRoleClaim>();
        }

        public AppRole(string roleName, string roleDescription = null!) : base(roleName)
        {
            RoleClaims = new HashSet<AppRoleClaim>();
            Description = roleDescription;
        }
    }
}
using BusinessLogic.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Entities.Identity
{
    public class AppRoleClaim : IdentityRoleClaim<string>, IAuditableEntity<int>
    {
        public string? Description { get; set; }
        public string? Group { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public virtual AppRole Role { get; set; } = default!;

        public AppRoleClaim() : base()
        {
        }

        public AppRoleClaim(string roleClaimDescription = null!, string roleClaimGroup = null!) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}
namespace BusinessLogic.Entities.Base
{
    public class AuditableBaseEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
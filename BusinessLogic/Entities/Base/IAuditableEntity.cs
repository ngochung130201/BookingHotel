namespace BusinessLogic.Entities.Base
{
    public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
    {
    }

    public interface IAuditableEntity : IEntity
    {
        string CreatedBy { get; set; }

        DateTime CreatedOn { get; set; }

        string? LastModifiedBy { get; set; }

        DateTime? LastModifiedOn { get; set; }

        bool IsDeleted { get; set; }
    }
}
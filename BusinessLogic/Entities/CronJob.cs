using BusinessLogic.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class CronJob : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string CronExpression { get; set; } = default!;

        [StringLength(50)]
        public string TimeZone { get; set; } = default!;

        public DateTime? LastRunTime { get; set; }

        public DateTime? NextRunTime { get; set; }

        public bool IsEnabled { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = default!;

        [MaxLength]
        public string? LastError { get; set; }
    }
}

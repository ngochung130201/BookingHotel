using System.ComponentModel.DataAnnotations;
using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class Contact : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(255)]
        public string? Subject { get; set; }
        [StringLength(255)]
        public string? Message { get; set; }
        [StringLength(15)]
        public string? PhoneNo { get; set; }
        [StringLength(255)] 
        public string? CompanyName { get; set; }
    }
}
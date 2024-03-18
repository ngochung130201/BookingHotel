using System.ComponentModel.DataAnnotations;
using BusinessLogic.Entities.Base;

namespace BusinessLogic.Entities
{
    public class ContactInfo : AuditableBaseEntity<int>
    {
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(15)]
        public string? PhoneNo { get; set; }
        [StringLength(255)] 
        public string? CompanyName { get; set; }
        public long Longitude { get; set; }
        public long Latitude { get; set; }
    }
}
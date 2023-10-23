using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.Models
{
    public class GroupType
    {
        [Key]
        public Guid GroupTypeId { get; set; }
        public Guid GroupTypeName { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class GroupType
    {
        [Key]
        public Guid GroupTypeId { get; set; }
        [Required(ErrorMessage = "Group Type is required")]
        [Column(TypeName = "varchar(25)")]
        public string GroupTypeName { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

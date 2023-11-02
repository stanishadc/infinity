using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class Group
    {
        [Key]
        public Guid GroupId { get; set; }
        [Column(TypeName = "varchar(45)")]
        public string GroupName { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string? Address { get; set; }
        public Guid? GroupTypeId { get; set; }
        public GroupType? GroupType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}
    }
}

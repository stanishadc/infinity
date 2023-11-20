using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.Models
{
    public class Infinity
    {
        [Key]
        public Guid InfinityId { get; set; }
        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? Address { get; set; }

        [Column(TypeName = "varchar(45)")]
        public string? PrimaryContact { get; set; }

        [Column(TypeName = "varchar(45)")]
        public string? Email { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string? Phone { get; set; }

        public string? CollectionNotes { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

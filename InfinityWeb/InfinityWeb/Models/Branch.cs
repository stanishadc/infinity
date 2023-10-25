using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class Branch
    {
        [Key]
        public Guid BranchId { get; set; }

        [Column(TypeName = "varchar(45)")]
        public string BranchName { get; set; }
        
        [Column(TypeName = "varchar(200)")]
        public string? Address { get; set; }
        
        [Column(TypeName = "varchar(45)")]
        public string? PrimaryContact { get; set; }
        
        [Column(TypeName = "varchar(45)")]
        public string? Email { get; set; }
        
        [Column(TypeName = "varchar(15)")]
        public string? Phone { get; set; }

        public Guid UserId { get; set; }

        public Guid GroupId {  get; set; }
        public Group Group { get; set; }
        public string? CollectionNotes { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}
    }
}

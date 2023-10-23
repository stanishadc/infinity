using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class Collector
    {
        [Key]
        public Guid CollectorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid BranchId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Address { get; set; }
        
        public Guid ItemId { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Requested { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Driver { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Collected { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Notes { get; set; }
    }
}

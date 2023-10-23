using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class Activity
    {
        [Key]
        public Guid ActivityId { get; set; }

        public Guid BranchId { get; set; }
        public Branch Branch { get; set; }

        public DateTime? ActivityDate { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string ItemName { get; set; }

        public double? QuantityRequested { get; set; }
        
        [Column(TypeName = "varchar(45)")]
        public string? RequestedBy { get; set; }

        [Column(TypeName = "varchar(45)")]
        public string? AllocatedTo { get; set; }

        public DateTime? AllocatedDate { get; set; }

        [Column(TypeName = "varchar(45)")]
        public string? Collected { get; set; }

        [Column(TypeName = "varchar(45)")]
        public string? Driver { get; set; }

        [Column(TypeName = "varchar(45)")]
        public string? DeliveredReceived { get; set; }
    }
}

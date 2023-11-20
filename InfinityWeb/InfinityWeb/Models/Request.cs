using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class Request
    {
        [Key]
        public Guid RequestId { get; set; }
        public Guid ItemId { get; set; }
        public double Quantity { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string? UserId { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        
        [Column(TypeName = "varchar(15)")]
        public string? RequestStatus { get; set; }
        
        public string? Notes { get; set; }
    }
}

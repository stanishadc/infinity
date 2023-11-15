using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class Item
    {
        public Guid ItemId { get; set; }
        [Required(ErrorMessage = "Item name is required")]
        [Column(TypeName = "varchar(100)")]
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set;}
        public double? CO2 { get; set;}        
        public double? OZone { get; set;}
        public DateTime UpdatedDate { get; set; }
    }
}

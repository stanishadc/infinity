using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class Item
    {
        public Guid ItemId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set;}
        [Column(TypeName = "varchar(25)")]
        public string? CO2 { get; set;}
        [Column(TypeName = "varchar(25)")]
        public string? OZone { get; set;}
    }
}

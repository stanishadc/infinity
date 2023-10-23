using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.Models
{
    public class Inventory
    {
        [Key]
        public Guid InventoryId { get; set; }
        public Guid ItemId { get; set; }
        public double? KgIn { get; set; }
        public double? KgOut { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}

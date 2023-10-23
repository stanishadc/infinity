using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.Models
{
    public class Production
    {
        [Key]
        public Guid ProductionId { get; set; }
        public DateTime ProductionDate { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get;set; }
        public double? Kg { get; set; }
    }
}

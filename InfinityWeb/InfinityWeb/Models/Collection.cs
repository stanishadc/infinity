using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.Models
{
    public class Collection
    {
        [Key]
        public Guid CollectionId { get; set; }
        public DateTime CollectionDate { get; set; }
        public Guid BranchId { get; set; }
        public Guid ItemId {  get; set; }
        public double? Quantity { get; set; }
        public DateTime RequestedDate { get; set; }
        public string? RequestedBy { get; set; }
    }
}

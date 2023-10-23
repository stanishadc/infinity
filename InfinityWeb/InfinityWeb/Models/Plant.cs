using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.Models
{
    public class Plant
    {
        [Key]
        public Guid PlantId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? Barcode { get; set; }
        public string? RequestData { get; set; }
        public double? ActualWeight { get; set; }
        public bool? ScannedToProcess { get; set; }
    }
}

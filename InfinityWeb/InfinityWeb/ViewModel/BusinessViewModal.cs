using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.ViewModel
{
    public class BusinessViewModal
    {
        [Key]
        public Guid BusinessId { get; set; }
        public string? PrimaryContact { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public Guid GroupId { get; set; }
        public string? GroupName { get; set; }
        public List<SelectListItem>? GroupsList { get; set; }

        public string? CollectionNotes { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime UpdatedDate { get; set; }
    }
}

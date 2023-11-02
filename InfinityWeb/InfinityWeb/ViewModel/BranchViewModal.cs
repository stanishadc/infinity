using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.ViewModel
{
    public class BranchViewModal
    {
        public Guid BranchId { get; set; }
        [Required(ErrorMessage = "Branch name is required")]
        public string BranchName { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "Primary Contact is required")]
        public string? PrimaryContact { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string? Phone { get; set; }
        public Guid GroupId { get; set; }
        public string? GroupName { get; set; }
        public List<SelectListItem>? GroupsList { get; set; }
        public string? CollectionNotes { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime UpdatedDate { get; set; }
    }
}

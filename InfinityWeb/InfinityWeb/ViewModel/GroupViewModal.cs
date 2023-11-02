using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.ViewModel
{
    public class GroupViewModal
    {
        public Guid GroupId { get; set; }
        [Required(ErrorMessage = "Group name is required")]
        public string GroupName { get; set; }
        public string? Address { get; set; }
        public Guid GroupTypeId { get; set; }
        public string? GroupTypeName { get; set; }
        public List<SelectListItem>? GroupTypesList { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime UpdatedDate { get; set; }
    }
}

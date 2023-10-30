using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InfinityWeb.Models;

namespace InfinityWeb.ViewModel
{
    public class GroupTypeViewModel
    {
        public Guid GroupTypeId { get; set; }
        [Required(ErrorMessage = "Group Type is required")]
        public string? GroupTypeName { get; set; }
        public DateTime? LastUpdated { get; set; }
        public List<GroupType>? GroupTypesList { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.ViewModel
{
    public class ClientBranchUserViewModal
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string? PhoneNumber { get; set; }
        public string? RoleName { get; set; }

        public Guid BranchId { get; set; }
        public string? BranchName { get; set; }
        public List<SelectListItem>? BranchsList { get; set; }

        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }

        public bool? IsActive { get;set; }
    }
}

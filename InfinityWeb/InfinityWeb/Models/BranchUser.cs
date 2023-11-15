using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityWeb.Models
{
    public class BranchUser : IdentityUser
    {
        [Column(TypeName = "varchar(45)")]
        public string? Name { get; set; }
        public Guid BranchId { get; set; }
        public bool? IsActive { get; set; }
    }
}

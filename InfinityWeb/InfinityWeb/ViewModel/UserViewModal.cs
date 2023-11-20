using System.ComponentModel.DataAnnotations;

namespace InfinityWeb.ViewModel
{
    public class UserViewModal
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class ChangePasswordModel
    {
        public string? Id { get; set; }
        public string? OldPassword { get; set; }        
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
    public class ConfirmEmailModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}

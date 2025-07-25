using System.ComponentModel.DataAnnotations;

namespace Login_Page.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name ="Remember me?")]
        public bool RememberMe { get; set; }

    }
}

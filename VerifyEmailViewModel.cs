using System.ComponentModel.DataAnnotations;

namespace Login_Page.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress]
        public required string EmailAddress { get; set; }
    }
}

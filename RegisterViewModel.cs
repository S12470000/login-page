using System.ComponentModel.DataAnnotations;

namespace Login_Page.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Name is Required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Phone Number is Required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress]
        public required string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
             ErrorMessage = "Password must be at least 8 characters long, contain at least 1 letter, 1 number and 1 special character.")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match. ")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required.")]
        [DataType(DataType.Password)]
        [Display(Name = " Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}

using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Email is Required.")]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is Required.")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = " New Password")]
    [Compare("ConfirmNewPassword", ErrorMessage = "Password does not match.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is Required.")]
    [DataType(DataType.Password)]
    [Display(Name = " Confirm New Password")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

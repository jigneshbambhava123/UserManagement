using System.ComponentModel.DataAnnotations;

namespace UserManagement.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    public int UserId {get; set; }

    [Required]
    public string Token { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [StringLength(12, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 12 characters")]
    public string NewPassword{ get; set; }

    [Required(ErrorMessage = "Confirm Password is required.")]
    [Compare("NewPassword",ErrorMessage = "Password do not match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword{ get; set; }
}

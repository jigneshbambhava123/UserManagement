using System.ComponentModel.DataAnnotations;

namespace UserManagement.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = " Email is required ")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email {get; set; }
}

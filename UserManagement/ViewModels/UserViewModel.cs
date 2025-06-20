using System.ComponentModel.DataAnnotations;

namespace UserManagement.ViewModels;

public class UserViewModel
{
     public int Id { get; set; }

    [Required(ErrorMessage = "First Name Required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters.")]
    [RegularExpression(@"^(?!.*[\x00-\x1F\x7F])[a-zA-Z0-9_]+(?: [a-zA-Z0-9_]+)*$", ErrorMessage = "First Name should only contain letters, numbers, underscores, and spaces between words. No leading/trailing spaces allowed.")]
    public string Firstname { get; set; } 

    [Required(ErrorMessage = "Last Name Required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters.")]
    [RegularExpression(@"^(?!.*[\x00-\x1F\x7F])[a-zA-Z0-9_]+(?: [a-zA-Z0-9_]+)*$", ErrorMessage = "Last Name should only contain letters, numbers, underscores, and spaces between words. No leading/trailing spaces allowed.")]
    public string Lastname { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } 

    [Required(ErrorMessage = "Password is required")]
    [StringLength(12, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 12 characters")]
    public string Password { get; set; } 

    public int RoleId { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public long? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    public DateTime Dateofbirth { get; set; }
}

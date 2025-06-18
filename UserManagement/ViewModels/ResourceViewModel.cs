using System.ComponentModel.DataAnnotations;

namespace UserManagement.ViewModels;

public class ResourceViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "String length must be between 2 and 50.")]
    [RegularExpression(@"^(?!.*[\x00-\x1F\x7F])[a-zA-Z0-9_]+(?: [a-zA-Z0-9_]+)*$", ErrorMessage = "Name should only contain letters, numbers, underscores, and spaces between words. No leading/trailing spaces allowed.")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    public int Quantity { get; set; }
}

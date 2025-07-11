using System.ComponentModel.DataAnnotations;

namespace UserManagement.ViewModels;

public class BookingViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Resource is required.")]
    public int ResourceId { get; set; }

    public int UserId { get; set; }
    
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "From Date is required.")]
    public DateTime FromDate { get; set; }

    [Required(ErrorMessage = "To Date is required.")]
    public DateTime ToDate { get; set; }

    public string? ResourceName { get; set; }

    public List<ResourceViewModel> Resources { get; set; } = new List<ResourceViewModel>();
}

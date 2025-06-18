using UserManagement.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IBookingService
{
    Task<(bool Success, string Message)> BookResourceAsync(BookingViewModel bookingViewModel);
    Task<IEnumerable<BookingViewModel>> GetActiveBookingsAsync(int userId);
    Task<IEnumerable<BookingViewModel>> GetBookingHistoryAsync(int userId);
}

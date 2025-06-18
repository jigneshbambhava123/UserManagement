using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly HttpClient _httpClient;

    public BookingService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<(bool Success, string Message)> BookResourceAsync(BookingViewModel bookingViewModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Booking/CreateBooking", bookingViewModel);
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

    public async Task<IEnumerable<BookingViewModel>> GetActiveBookingsAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingViewModel>>($"api/Booking/GetActiveBookings?userId={userId}");
    }

    public async Task<IEnumerable<BookingViewModel>> GetBookingHistoryAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingViewModel>>($"api/Booking/GetBookingHistory?userId={userId}");
    }
}

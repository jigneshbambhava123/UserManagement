using UserManagement.Services.Base;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Services.Implementations;

public class BookingService :  BaseService, IBookingService
{
    public BookingService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    : base(httpClientFactory.CreateClient("ApiClient"), httpContextAccessor) 
    {}

    public async Task<(bool Success, string Message)> BookResourceAsync(BookingViewModel bookingViewModel)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/Booking", bookingViewModel);
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

    public async Task<IEnumerable<BookingViewModel>> GetActiveBookingsAsync(int userId)
    {
        SetAuthorizationHeader();
        return await HttpClient.GetFromJsonAsync<IEnumerable<BookingViewModel>>($"api/Booking/ActiveBookings?id={userId}");
    }

    public async Task<IEnumerable<BookingViewModel>> GetBookingHistoryAsync(int userId)
    {
        SetAuthorizationHeader();
        return await HttpClient.GetFromJsonAsync<IEnumerable<BookingViewModel>>($"api/Booking/ResourceHistory?id={userId}");
    }

    public async Task<int> GetTotalActiveUserCount()
    {
        SetAuthorizationHeader();
        var allActiveBookings = await HttpClient.GetFromJsonAsync<IEnumerable<BookingViewModel>>("api/Booking/ActiveBookings");

        if (allActiveBookings == null)
        {
            return 0;
        }

        var distinctActiveUsers = allActiveBookings.Select(b => b.UserId).Distinct().Count();
        return distinctActiveUsers;
    }
}

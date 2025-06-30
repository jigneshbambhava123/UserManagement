using System.Net.Http.Json;
using System.Text.Json;
using System.Net;
using UserManagement.Services.Base;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;
using System.Collections.Generic; 

namespace UserManagement.Services.Implementations;

public class BookingService : BaseService, IBookingService
{
    public BookingService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    : base(httpClientFactory.CreateClient("ApiClient"), httpContextAccessor)
    {}

    public async Task<(bool Success, string Message)> BookResourceAsync(BookingViewModel bookingViewModel)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/Booking", bookingViewModel);
        return await HandleApiTupleResponse(response); 
    }

    public async Task<IEnumerable<BookingViewModel>> GetActiveBookingsAsync(int userId)
    {
        SetAuthorizationHeader();
        HttpResponseMessage response = await HttpClient.GetAsync($"api/Booking/ActiveBookings?id={userId}");
        await HandleApiVoidResponse(response); 

        var bookings = await response.Content.ReadFromJsonAsync<IEnumerable<BookingViewModel>>();
        return bookings ?? Enumerable.Empty<BookingViewModel>();
    }

    public async Task<IEnumerable<BookingViewModel>> GetBookingHistoryAsync(int userId)
    {
        SetAuthorizationHeader();
        HttpResponseMessage response = await HttpClient.GetAsync($"api/Booking/ResourceHistory?id={userId}");
        await HandleApiVoidResponse(response); 

        var bookings = await response.Content.ReadFromJsonAsync<IEnumerable<BookingViewModel>>();
        return bookings ?? Enumerable.Empty<BookingViewModel>();
    }

    public async Task<int> GetTotalActiveUserCount()
    {
        SetAuthorizationHeader();
        HttpResponseMessage response = await HttpClient.GetAsync("api/Booking/ActiveBookings");
        await HandleApiVoidResponse(response); 

        var allActiveBookings = await response.Content.ReadFromJsonAsync<IEnumerable<BookingViewModel>>();

        if (allActiveBookings == null)
        {
            return 0;
        }

        var distinctActiveUsers = allActiveBookings.Select(b => b.UserId).Distinct().Count();
        return distinctActiveUsers;
    }
}
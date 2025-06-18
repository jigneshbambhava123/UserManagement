
namespace UserManagement.BackgroundServices;

public class ExpiredBookingsService :BackgroundService
{
    private readonly HttpClient _httpClient;

    public ExpiredBookingsService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
                var response = await _httpClient.PostAsync("api/Booking/ReleaseExpiredBookings", null, stoppingToken); 
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                // await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }
}

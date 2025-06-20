using System.Text.Json;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Services.Implementations;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<UserViewModel>>("api/User");
    }

    public async Task<UserViewModel> GetUserByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<UserViewModel>($"api/User/{id}");
    }

    public async Task<(bool Success, string Message)> CreateUserAsync(UserViewModel user)
    {
        var response = await _httpClient.PostAsJsonAsync("api/User", user);
        var message = await response.Content.ReadAsStringAsync();

        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> UpdateUserAsync(UserViewModel user)
    {
        var response = await _httpClient.PutAsJsonAsync("api/User", user);
        var message = await response.Content.ReadAsStringAsync();

        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/User?id={id}");
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

}

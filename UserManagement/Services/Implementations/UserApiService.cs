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

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<User>>("api/User/GetUsers");
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<User>($"api/User/GetUserById/{id}");
    }

    public async Task<(bool Success, string Message)> CreateUserAsync(User user)
    {
        var response = await _httpClient.PostAsJsonAsync("api/User/CreateUser", user);
        var message = await response.Content.ReadAsStringAsync();

        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> UpdateUserAsync(User user)
    {
        var response = await _httpClient.PutAsJsonAsync("api/User/UpdateUser", user);
        var message = await response.Content.ReadAsStringAsync();

        return (response.IsSuccessStatusCode, message);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/User/DeleteUser/{id}");
        return response.IsSuccessStatusCode;
    }

}

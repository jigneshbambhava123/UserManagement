using System.Text.Json;
using UserManagement.Services.Base;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Services.Implementations;

public class UserApiService :  BaseService, IUserApiService
{
    public UserApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    : base(httpClientFactory.CreateClient("ApiClient"), httpContextAccessor) 
    {}

    public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
    {
        SetAuthorizationHeader();
        return await HttpClient.GetFromJsonAsync<IEnumerable<UserViewModel>>("api/User");
    }

    public async Task<UserViewModel> GetUserByIdAsync(int id)
    {
        SetAuthorizationHeader();
        return await HttpClient.GetFromJsonAsync<UserViewModel>($"api/User/{id}");
    }

    public async Task<(bool Success, string Message)> CreateUserAsync(UserViewModel user)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/User", user);
        var message = await response.Content.ReadAsStringAsync();

        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> UpdateUserAsync(UserViewModel user)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PutAsJsonAsync("api/User", user);
        var message = await response.Content.ReadAsStringAsync();

        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.DeleteAsync($"api/User?id={id}");
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

}

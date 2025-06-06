using UserManagement.ViewModels;

namespace UserManagement.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // _httpClient.BaseAddress = new Uri("http://localhost:5086/");   
    }

   public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<User>>("api/User/all");
    }


    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<User>($"api/User/{id}");
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        var response = await _httpClient.PostAsJsonAsync("api/User/create", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var response = await _httpClient.PutAsJsonAsync("api/User/update", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/User/delete/{id}");
        return response.IsSuccessStatusCode;
    }
}

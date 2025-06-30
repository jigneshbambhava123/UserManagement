using System.Net.Http.Json;
using System.Text.Json; 
using UserManagement.Services.Base;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;
using System.Net;

namespace UserManagement.Services.Implementations;

public class UserApiService : BaseService, IUserApiService
{
    public UserApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    : base(httpClientFactory.CreateClient("ApiClient"), httpContextAccessor)
    {}

    public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
    {
        SetAuthorizationHeader();
        HttpResponseMessage response = await HttpClient.GetAsync("api/User");
        await HandleApiVoidResponse(response); 

        var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserViewModel>>();
        return users ?? Enumerable.Empty<UserViewModel>();
    }

    public async Task<UserViewModel?> GetUserByIdAsync(int id)
    {
        SetAuthorizationHeader();
        HttpResponseMessage response = await HttpClient.GetAsync($"api/User/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await HandleApiVoidResponse(response); 

        return await response.Content.ReadFromJsonAsync<UserViewModel>();
    }

    public async Task<(bool Success, string Message)> CreateUserAsync(UserViewModel user)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/User", user);
        return await HandleApiTupleResponse(response); 
    }

    public async Task<(bool Success, string Message)> UpdateUserAsync(UserViewModel user)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PutAsJsonAsync("api/User", user);
        return await HandleApiTupleResponse(response);
    }

    public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.DeleteAsync($"api/User?id={id}");
        return await HandleApiTupleResponse(response); 
    }
}


using System.Text.Json;
using UserManagement.Services.Base;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Services.Implementations;

public class AuthService: BaseService,IAuthService
{

    public AuthService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    : base(httpClientFactory.CreateClient("ApiClient"), httpContextAccessor)
    {}

    public Task<HttpResponseMessage> LoginAsync(LoginViewModel loginModel)
    {
        return HttpClient.PostAsJsonAsync("api/Account/Login", loginModel);
    }

    public async Task<(bool Success, string Token, int UserId, string Message)> ForgotPasswordAsync(string email, string baseUrl)
    {

        var response = await HttpClient.PostAsync($"api/Account/ForgotPassword?email={email}&baseUrl={baseUrl}", null);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
            var token = json.GetValueOrDefault("token").GetString();
            int userId = json.GetValueOrDefault("userId").GetInt32();
            var message = json.GetValueOrDefault("message").GetString();

            return (true, token, userId, message);
        }
        else
        {
            var (success, message) = await HandleApiTupleResponse(response);
            return (false, null, 0, message);
        }
    }


    public async Task<bool> ValidateResetTokenAsync(int userId, string token)
    {
        var response = await HttpClient.GetAsync($"api/Account?userId={userId}&token={token}");
        return response.IsSuccessStatusCode;
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        var response = await HttpClient.PostAsJsonAsync("api/Account/ResetPassword", model);

        await HandleApiVoidResponse(response);

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<(bool Success, string Message)> RegisterAsync(UserViewModel user)
    {

        var response = await HttpClient.PostAsJsonAsync("api/Account/Register", user);

        return await HandleApiTupleResponse(response);
    }

}

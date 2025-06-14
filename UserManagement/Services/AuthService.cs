using System.Text.Json;
using UserManagement.ViewModels;

namespace UserManagement.Services;

public class AuthService: IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> LoginAsync(LoginViewModel loginModel)
    {
        return _httpClient.PostAsJsonAsync("api/Account/login", loginModel);
    }

    public async Task<(bool Success, string Token, int UserId, string Message)> ForgotPasswordAsync(string email, string baseUrl)
    {
        var response = await _httpClient.PostAsync($"api/Account/forgot-password?email={email}&baseUrl={baseUrl}", null);
        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();


        var token = json["token"].GetString();
        int userId = json["userId"].GetInt32();
        var message = json["message"].GetString();

        if (userId==0)
            return (false, null, 0, json?["message"].GetString());

        return (true, token, userId, message);
    }

    public async Task<bool> ValidateResetTokenAsync(int userId, string token)
    {
        var response = await _httpClient.GetAsync($"api/Account/validate-reset-token?userId={userId}&token={token}");
        return response.IsSuccessStatusCode;
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Account/reset-password", model);

        var message = await response.Content.ReadAsStringAsync();
        return message;
    }


}

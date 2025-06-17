using UserManagement.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IAuthService
{
    Task<HttpResponseMessage> LoginAsync(LoginViewModel loginModel);
    Task<(bool Success, string Token, int UserId, string Message)> ForgotPasswordAsync(string email, string baseUrl);
    Task<bool> ValidateResetTokenAsync(int userId, string token);
    Task<string> ResetPasswordAsync(ResetPasswordViewModel model);
}

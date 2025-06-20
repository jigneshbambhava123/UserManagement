using UserManagement.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IUserApiService
{
    Task<IEnumerable<UserViewModel>> GetAllUsersAsync();
    Task<UserViewModel?> GetUserByIdAsync(int id);
    Task<(bool Success, string Message)> CreateUserAsync(UserViewModel user);
    Task<(bool Success, string Message)> UpdateUserAsync(UserViewModel user);
    Task<(bool Success, string Message)> DeleteUserAsync(int id);
}

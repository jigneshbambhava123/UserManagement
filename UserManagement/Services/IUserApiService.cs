using UserManagement.ViewModels;

namespace UserManagement.Services;

public interface IUserApiService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<(bool Success, string Message)> CreateUserAsync(User user);
    Task<(bool Success, string Message)> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}

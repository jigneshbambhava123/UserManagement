using UserManagement.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IResourceService
{
    Task<IEnumerable<ResourceViewModel>> GetAllResourcesAsync();
    Task<ResourceViewModel?> GetResourceByIdAsync(int id);
    Task<(bool Success, string Message)> CreateResourceAsync(ResourceViewModel resource);
    Task<(bool Success, string Message)> UpdateResourceAsync(ResourceViewModel resource);
    Task<(bool Success, string Message)> DeleteResourceAsync(int id);
}

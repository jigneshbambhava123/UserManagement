using UserManagement.Services.Base;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Services.Implementations;

public class ResourceService :  BaseService, IResourceService
{
    public ResourceService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    : base(httpClientFactory.CreateClient("ApiClient"), httpContextAccessor) 
    {}

    public async Task<IEnumerable<ResourceViewModel>> GetAllResourcesAsync()
    {
        SetAuthorizationHeader();
        return await HttpClient.GetFromJsonAsync<IEnumerable<ResourceViewModel>>("api/Resource");
    }

    public async Task<ResourceViewModel?> GetResourceByIdAsync(int id)
    {
        SetAuthorizationHeader();
        return await HttpClient.GetFromJsonAsync<ResourceViewModel>($"api/Resource/{id}");
    }
    
    public async Task<(bool Success, string Message)> CreateResourceAsync(ResourceViewModel resource)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/Resource", resource);
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> UpdateResourceAsync(ResourceViewModel resource)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PutAsJsonAsync("api/Resource", resource);
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> DeleteResourceAsync(int id)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.DeleteAsync($"api/Resource?id={id}");
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }
}

using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Services.Implementations;

public class ResourceService : IResourceService
{
    private readonly HttpClient _httpClient;

    public ResourceService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<IEnumerable<ResourceViewModel>> GetAllResourcesAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<ResourceViewModel>>("api/Resource/GetResources");
    }

    public async Task<ResourceViewModel?> GetResourceByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ResourceViewModel>($"api/Resource/GetResourceById?id={id}");
    }
    
    public async Task<(bool Success, string Message)> CreateResourceAsync(ResourceViewModel resource)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Resource/CreateResource", resource);
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> UpdateResourceAsync(ResourceViewModel resource)
    {
        var response = await _httpClient.PutAsJsonAsync("api/Resource/UpdateResource", resource);
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

    public async Task<(bool Success, string Message)> DeleteResourceAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/Resource/DeleteResource?id={id}");
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }
}

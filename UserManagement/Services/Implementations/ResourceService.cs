using UserManagement.Services.Base;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;
using System.Net.Http.Json; 
using System.Text.Json;     
using System.Net;         
using System.Collections.Generic; 

namespace UserManagement.Services.Implementations;

public class ResourceService : BaseService, IResourceService 
{
    public ResourceService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    : base(httpClientFactory.CreateClient("ApiClient"), httpContextAccessor)
    {}

    public async Task<IEnumerable<ResourceViewModel>> GetAllResourcesAsync()
    {
        SetAuthorizationHeader();
        HttpResponseMessage response = await HttpClient.GetAsync("api/Resource");
        await HandleApiVoidResponse(response); 

        var resources = await response.Content.ReadFromJsonAsync<IEnumerable<ResourceViewModel>>();
        return resources ?? Enumerable.Empty<ResourceViewModel>();
    }

    public async Task<ResourceViewModel?> GetResourceByIdAsync(int id)
    {
        SetAuthorizationHeader();
        HttpResponseMessage response = await HttpClient.GetAsync($"api/Resource/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await HandleApiVoidResponse(response); 

        return await response.Content.ReadFromJsonAsync<ResourceViewModel>();
    }

    public async Task<(bool Success, string Message)> CreateResourceAsync(ResourceViewModel resource)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/Resource", resource);
        return await HandleApiTupleResponse(response);
    }

    public async Task<(bool Success, string Message)> UpdateResourceAsync(ResourceViewModel resource)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.PutAsJsonAsync("api/Resource", resource);
        return await HandleApiTupleResponse(response);
    }

    public async Task<(bool Success, string Message)> UpdateResourceFieldAsync(int id, Dictionary<string, string> updateData)
    {
        SetAuthorizationHeader();
        var content = JsonContent.Create(updateData);
        var response = await HttpClient.PatchAsync($"api/Resource?id={id}", content);
        return await HandleApiTupleResponse(response); 
    }

    public async Task<(bool Success, string Message)> DeleteResourceAsync(int id)
    {
        SetAuthorizationHeader();
        var response = await HttpClient.DeleteAsync($"api/Resource?id={id}");
        return await HandleApiTupleResponse(response);
    }
}
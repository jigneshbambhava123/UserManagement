using System.Net.Http.Headers;

namespace UserManagement.Services.Base;

public abstract class BaseService
{
    protected readonly HttpClient HttpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        HttpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    protected void SetAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.User?.FindFirst("access_token")?.Value;
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("JWT token is missing from session.");
        }

        if (HttpClient.DefaultRequestHeaders.Authorization?.Parameter != token)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}


using System.Net.Http.Headers;
using System.Text.Json;

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

    protected async Task HandleApiVoidResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            string errorMessage = errorContent;

            try
            {
                var apiError = JsonSerializer.Deserialize<ViewModels.ApiErrorResponse>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (apiError != null)
                {
                    if (!string.IsNullOrEmpty(apiError.Message))
                    {
                        errorMessage = apiError.Message;
                    }
                    else if (!string.IsNullOrEmpty(apiError.Details))
                    {
                        errorMessage = apiError.Details;
                    }
                }
            }
            catch (JsonException)
            {
            }
            throw new HttpRequestException($"API call failed: {response.StatusCode} - {errorMessage}", null, response.StatusCode);
        }
    }

    protected async Task<(bool Success, string Message)> HandleApiTupleResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return (true, await response.Content.ReadAsStringAsync());
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                var apiError = JsonSerializer.Deserialize<ViewModels.ApiErrorResponse>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (apiError != null)
                {
                    if (!string.IsNullOrEmpty(apiError.Message))
                    {
                        return (false, apiError.Message);
                    }
                }
                return (false, $"An API error occurred: {response.StatusCode} - {errorContent}");
            }
            catch (JsonException)
            {
                return (false, $"An unexpected error occurred: {response.StatusCode} - {errorContent}");
            }
        }
    }
}


using System.Text.Json;

namespace Internship.Blazor.Services;

public partial class ApiService(HttpClient httpClient, ILogger<ApiService> _logger)
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
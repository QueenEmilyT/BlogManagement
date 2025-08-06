using System.Net.Http.Json;
using Internship.Blazor.Exceptions;

namespace Internship.Blazor.Services;

public partial class ApiService
{
    public async Task<TResponse> GetAsync<TResponse>(
    string url,
    CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

    try
    {
        _logger.LogInformation("Sending GET request to {Url}", url);
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        _logger.LogInformation("Received response with status code {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("GET request to {Url} failed with status code {StatusCode}. Response: {Response}", 
                url, response.StatusCode, errorContent);

            var problemDetails = await ParseProblemDetails(errorContent, response.StatusCode);
            throw new ApiException<ProblemDetails>((int)response.StatusCode, problemDetails, errorContent);
        }

        var result = await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken)
                     ?? throw new InvalidOperationException("Deserialized response is null.");

        _logger.LogInformation("Successfully deserialized response from {Url}", url);
        return result;
    }
    catch (ApiException)
    {
        throw; // Re-throw our custom exceptions
    }
    catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
    {
        _logger.LogError(ex, "GET request to {Url} timed out.", url);
        var problemDetails = CreateTimeoutProblemDetails(url);
        throw new ApiException<ProblemDetails>(408, problemDetails);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "HTTP request error while sending GET request to {Url}", url);
        var problemDetails = CreateConnectionProblemDetails();
        throw new ApiException<ProblemDetails>(0, problemDetails);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while sending GET request to {Url}", url);
        var problemDetails = CreateUnexpectedErrorProblemDetails();
        throw new ApiException<ProblemDetails>(500, problemDetails);
    }
}
}
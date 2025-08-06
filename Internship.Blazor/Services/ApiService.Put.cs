using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Internship.Blazor.Exceptions;

namespace Internship.Blazor.Services;

public partial class ApiService
{
    public async Task<TResponse> PutAsync<TRequest, TResponse>(
    string url,
    TRequest payload,
    CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

    if (payload == null)
        throw new ArgumentNullException(nameof(payload), "Payload cannot be null.");

    try
    {
        _logger.LogInformation("Sending PUT request to {Url}", url);
        var jsonPayload = JsonSerializer.Serialize(payload, _jsonOptions);
        using var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PutAsync(url, requestContent, cancellationToken);
        _logger.LogInformation("Received response with status code {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("PUT request to {Url} failed with status code {StatusCode}. Response: {Response}",
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
        throw;
    }
    catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
    {
        _logger.LogError(ex, "PUT request to {Url} timed out.", url);
        var problemDetails = CreateTimeoutProblemDetails(url);
        throw new ApiException<ProblemDetails>(408, problemDetails);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "HTTP request error while sending PUT request to {Url}", url);
        var problemDetails = CreateConnectionProblemDetails();
        throw new ApiException<ProblemDetails>(0, problemDetails);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while sending PUT request to {Url}", url);
        var problemDetails = CreateUnexpectedErrorProblemDetails();
        throw new ApiException<ProblemDetails>(500, problemDetails);
    }
}

public async Task<T> PutAsync<T>(
    string url,
    T payload,
    CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

    if (payload == null)
        throw new ArgumentNullException(nameof(payload), "Payload cannot be null.");

    try
    {
        _logger.LogInformation("Sending PUT request to {Url}", url);
        var jsonPayload = JsonSerializer.Serialize(payload, _jsonOptions);
        using var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PutAsync(url, requestContent, cancellationToken);
        _logger.LogInformation("Received response with status code {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("PUT request to {Url} failed with status code {StatusCode}. Response: {Response}",
                url, response.StatusCode, errorContent);

            var problemDetails = await ParseProblemDetails(errorContent, response.StatusCode);
            throw new ApiException<ProblemDetails>((int)response.StatusCode, problemDetails, errorContent);
        }

        var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken)
                     ?? throw new InvalidOperationException("Deserialized response is null.");

        _logger.LogInformation("Successfully deserialized response from {Url}", url);
        return result;
    }
    catch (ApiException)
    {
        throw;
    }
    catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
    {
        _logger.LogError(ex, "PUT request to {Url} timed out.", url);
        var problemDetails = CreateTimeoutProblemDetails(url);
        throw new ApiException<ProblemDetails>(408, problemDetails);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "HTTP request error while sending PUT request to {Url}", url);
        var problemDetails = CreateConnectionProblemDetails();
        throw new ApiException<ProblemDetails>(0, problemDetails);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while sending PUT request to {Url}", url);
        var problemDetails = CreateUnexpectedErrorProblemDetails();
        throw new ApiException<ProblemDetails>(500, problemDetails);
    }
}
}
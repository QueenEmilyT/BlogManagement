using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Internship.Blazor.Exceptions;

namespace Internship.Blazor.Services;

public partial class ApiService
{
    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string url,
        TRequest payload,
        CancellationToken cancellationToken = default)
    {
        ValidateInputs(url, payload);

        try
        {
            _logger.LogDebug("Sending POST request to {Url}", url);
            
            var response = await SendPostRequestAsync(url, payload, cancellationToken);
            var result = await HandleSuccessResponseAsync<TResponse>(response, url, cancellationToken);
            
            return result;
        }
        catch (ApiException)
        {
            // Re-throw ApiException as-is to preserve original error details
            throw;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("POST request to {Url} was cancelled by user", url);
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "POST request to {Url} timed out", url);
            var problemDetails = CreateTimeoutProblemDetails(url);
            throw new ApiException<ProblemDetails>(408, problemDetails);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error while sending POST request to {Url}: {Message}", url, ex.Message);
            var problemDetails = CreateConnectionProblemDetails(ex.Message);
            throw new ApiException<ProblemDetails>(0, problemDetails);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON serialization/deserialization error for POST request to {Url}", url);
            var problemDetails = CreateSerializationProblemDetails();
            throw new ApiException<ProblemDetails>(422, problemDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while sending POST request to {Url}: {Message}", url, ex.Message);
            var problemDetails = CreateUnexpectedErrorProblemDetails();
            throw new ApiException<ProblemDetails>(500, problemDetails);
        }
    }

    public async Task<T> PostAsync<T>(
        string url,
        T payload,
        CancellationToken cancellationToken = default)
    {
        ValidateInputs(url, payload);

        try
        {
            _logger.LogDebug("Sending POST request to {Url}", url);
            
            var response = await SendPostRequestAsync(url, payload, cancellationToken);
            var result = await HandleSuccessResponseAsync<T>(response, url, cancellationToken);
            
            return result;
        }
        catch (ApiException)
        {
            // Re-throw ApiException as-is to preserve original error details
            throw;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("POST request to {Url} was cancelled by user", url);
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "POST request to {Url} timed out", url);
            var problemDetails = CreateTimeoutProblemDetails(url);
            throw new ApiException<ProblemDetails>(408, problemDetails);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error while sending POST request to {Url}: {Message}", url, ex.Message);
            var problemDetails = CreateConnectionProblemDetails(ex.Message);
            throw new ApiException<ProblemDetails>(0, problemDetails);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON serialization/deserialization error for POST request to {Url}", url);
            var problemDetails = CreateSerializationProblemDetails();
            throw new ApiException<ProblemDetails>(422, problemDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while sending POST request to {Url}: {Message}", url, ex.Message);
            var problemDetails = CreateUnexpectedErrorProblemDetails();
            throw new ApiException<ProblemDetails>(500, problemDetails);
        }
    }

    private static void ValidateInputs<T>(string url, T payload)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        if (payload == null)
            throw new ArgumentNullException(nameof(payload), "Payload cannot be null.");
    }

    private async Task<HttpResponseMessage> SendPostRequestAsync<T>(
        string url, 
        T payload, 
        CancellationToken cancellationToken)
    {
        var jsonPayload = JsonSerializer.Serialize(payload, _jsonOptions);
        using var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(url, requestContent, cancellationToken);
        
        _logger.LogDebug("Received response from {Url} with status code {StatusCode}", url, response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response, url, cancellationToken);
        }

        return response;
    }

    private async Task<T> HandleSuccessResponseAsync<T>(
        HttpResponseMessage response, 
        string url, 
        CancellationToken cancellationToken)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
        
        if (result == null)
        {
            _logger.LogWarning("Received null response from {Url} despite success status code", url);
            throw new InvalidOperationException($"Received null response from {url}");
        }

        _logger.LogDebug("Successfully processed response from {Url}", url);
        return result;
    }

    private async Task HandleErrorResponseAsync(
        HttpResponseMessage response, 
        string url, 
        CancellationToken cancellationToken)
    {
        var statusCode = response.StatusCode;
        
        // Fast path for validation errors - avoid expensive operations
        if (IsValidationError(statusCode))
        {
            // Don't read the entire content if we can avoid it for large payloads
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            // Minimal logging for validation errors to reduce overhead
            _logger.LogDebug("Validation error for request to {Url}: {StatusCode}", 
                url, statusCode);
            
            // Create problem details with minimal processing
            var details = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = statusCode == HttpStatusCode.BadRequest ? "Validation Error" : "Business Rule Error",
                Detail = errorContent,
                Type = $"https://httpstatuses.com/{(int)statusCode}"
            };
            
            throw new ApiException<ProblemDetails>((int)statusCode, details, errorContent);
        }
        
        // Standard path for other errors
        var fullErrorContent = await response.Content.ReadAsStringAsync(cancellationToken);
        
        // Log different levels based on status code
        if (IsClientError(statusCode))
        {
            _logger.LogWarning("Client error for POST request to {Url}: {StatusCode} - {Response}", 
                url, statusCode, SanitizeErrorContent(fullErrorContent));
        }
        else
        {
            _logger.LogError("Server error for POST request to {Url}: {StatusCode} - {Response}", 
                url, statusCode, SanitizeErrorContent(fullErrorContent));
        }

        var problemDetails = await ParseProblemDetailsAsync(fullErrorContent, statusCode);
        throw new ApiException<ProblemDetails>((int)statusCode, problemDetails, fullErrorContent);
    }
}
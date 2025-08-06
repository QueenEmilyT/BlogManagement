using Internship.Blazor.Exceptions;

namespace Internship.Blazor.Services;

public partial class ApiService
{
     public async Task DeleteAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

        try
        {
            _logger.LogInformation("Sending DELETE request to {Url}", url);

            using var response = await _httpClient.DeleteAsync(url, cancellationToken);
            _logger.LogInformation("Received response with status code {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("DELETE request to {Url} failed with status code {StatusCode}. Response: {Response}",
                    url, response.StatusCode, errorContent);

                // Use the shared helper method to consistently parse ProblemDetails
                var problemDetails = await ParseProblemDetails(errorContent, response.StatusCode);
                throw new ApiException<ProblemDetails>((int)response.StatusCode, problemDetails, errorContent);
            }

            _logger.LogInformation("Successfully completed DELETE request to {Url}", url);
        }
        catch (ApiException)
        {
            // Re-throw our custom exceptions
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "DELETE request to {Url} timed out.", url);
            var problemDetails = CreateTimeoutProblemDetails(url);
            throw new ApiException<ProblemDetails>(408, problemDetails);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error while sending DELETE request to {Url}", url);
            var problemDetails = CreateConnectionProblemDetails();
            throw new ApiException<ProblemDetails>(0, problemDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while sending DELETE request to {Url}", url);
            var problemDetails = CreateUnexpectedErrorProblemDetails();
            throw new ApiException<ProblemDetails>(500, problemDetails);
        }
    }

}
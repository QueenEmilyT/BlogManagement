using System.Net;
using System.Text.Json;
using Internship.Blazor.Exceptions;

namespace Internship.Blazor.Services;

/// <summary>
/// Helper utility methods for the ApiService class to handle errors and parse responses
/// </summary>
public partial class ApiService
{
    /// <summary>
    /// Sanitizes error content for logging purposes by truncating if too long
    /// </summary>
    private string SanitizeErrorContent(string content)
    {
        if (string.IsNullOrEmpty(content))
            return "[Empty]";

        // Truncate long error content in logs
        const int maxLength = 500;
        if (content.Length > maxLength)
            return content.Substring(0, maxLength) + "... [truncated]";
            
        return content;
    }

    /// <summary>
    /// Determines if a status code represents a client error (400-499)
    /// </summary>
    private bool IsClientError(HttpStatusCode statusCode)
    {
        var code = (int)statusCode;
        return code >= 400 && code < 500;
    }

    /// <summary>
    /// Determines if an error is a validation error that should be handled specially
    /// </summary>
    private bool IsValidationError(HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.BadRequest || 
               statusCode == (HttpStatusCode)422; // Unprocessable Entity
    }

    /// <summary>
    /// Parses error content into a ProblemDetails object for validation errors
    /// This method is optimized to handle common validation error formats quickly
    /// </summary>
    private async Task<ProblemDetails> ParseProblemDetailsAsync(string errorContent, HttpStatusCode statusCode)
    {
        // Fast path for validation errors (400, 422) - don't do expensive processing
        if (IsValidationError(statusCode))
        {
            try
            {
                // Try quick deserialize without complex parsing for validation errors
                if (!string.IsNullOrWhiteSpace(errorContent))
                {
                    // For known validation error format, create basic problem details
                    return new ProblemDetails
                    {
                        Status = (int)statusCode,
                        Title = statusCode == HttpStatusCode.BadRequest ? "Validation Error" : "Business Rule Error",
                        Detail = errorContent,
                        Type = $"https://httpstatuses.com/{(int)statusCode}"
                    };
                }
            }
            catch
            {
                // Fall through to standard parsing if quick path fails
            }
        }

        // For other errors or if fast path failed, do standard parsing
        return await ParseProblemDetails(errorContent, statusCode);
    }

    /// <summary>
    /// Creates a ProblemDetails object for timeout errors
    /// </summary>
    private ProblemDetails CreateTimeoutProblemDetails(string url)
    {
        return new ProblemDetails
        {
            Status = 408,
            Title = "Request Timeout",
            Detail = $"The request to {url} timed out. Please try again later.",
            Type = "https://httpstatuses.com/408"
        };
    }

    /// <summary>
    /// Creates a ProblemDetails object for connection errors
    /// </summary>
    private ProblemDetails CreateConnectionProblemDetails(string? detail = null)
    {
        return new ProblemDetails
        {
            Status = 503,
            Title = "Connection Error",
            Detail = detail ?? "Unable to connect to the server. Please check your network connection and try again.",
            Type = "https://httpstatuses.com/503"
        };
    }

    /// <summary>
    /// Creates a ProblemDetails object for unexpected errors
    /// </summary>
    private ProblemDetails CreateUnexpectedErrorProblemDetails()
    {
        return new ProblemDetails
        {
            Status = 500,
            Title = "Unexpected Error",
            Detail = "An unexpected error occurred. Please try again later.",
            Type = "https://httpstatuses.com/500"
        };
    }

    /// <summary>
    /// Creates a ProblemDetails object for serialization errors
    /// </summary>
    private ProblemDetails CreateSerializationProblemDetails()
    {
        return new ProblemDetails
        {
            Status = 422,
            Title = "Serialization Error",
            Detail = "Could not process the request or response data.",
            Type = "https://httpstatuses.com/422"
        };
    }

    /// <summary>
    /// Main implementation of problem details parsing that can handle different error formats
    /// </summary>
    private async Task<ProblemDetails> ParseProblemDetails(string errorContent, HttpStatusCode statusCode)
    {
        if (string.IsNullOrWhiteSpace(errorContent))
        {
            return new ProblemDetails
            {
                Status = (int)statusCode,
                Title = $"HTTP {(int)statusCode} Error",
                Detail = "No additional details available",
                Type = $"https://httpstatuses.com/{(int)statusCode}"
            };
        }

        try
        {
            // Try to deserialize as a ProblemDetails object
            var options = new JsonSerializerOptions(_jsonOptions) { PropertyNameCaseInsensitive = true };
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(errorContent, options);

            if (problemDetails != null)
            {
                // Fill in missing fields if needed
                problemDetails.Status ??= (int)statusCode;
                problemDetails.Title ??= $"HTTP {(int)statusCode} Error";
                problemDetails.Type ??= $"https://httpstatuses.com/{(int)statusCode}";
                
                return problemDetails;
            }
        }
        catch (JsonException)
        {
            // If it's not valid JSON or doesn't match the ProblemDetails format,
            // we'll create a new ProblemDetails with the raw content as the detail
            _logger.LogDebug("Error content is not a valid ProblemDetails JSON");
        }

        // Fallback when deserializing fails or returns null
        return new ProblemDetails
        {
            Status = (int)statusCode,
            Title = $"HTTP {(int)statusCode} Error",
            Detail = errorContent,
            Type = $"https://httpstatuses.com/{(int)statusCode}"
        };
    }
}

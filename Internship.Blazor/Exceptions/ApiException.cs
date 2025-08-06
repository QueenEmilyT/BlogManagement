namespace Internship.Blazor.Exceptions;

public class ApiException(int statusCode, string? responseContent = null, string? message = null)
    : Exception(message ?? $"API request failed with status code {statusCode}")
{
    public int StatusCode { get; } = statusCode;
    public string? ResponseContent { get; } = responseContent;
}


public class ApiException<T>(
    int statusCode,
    T? errorData = null,
    string? responseContent = null,
    string? message = null)
    : ApiException(statusCode, responseContent, message)
    where T : class
{
    public T? ErrorData { get; } = errorData;
}
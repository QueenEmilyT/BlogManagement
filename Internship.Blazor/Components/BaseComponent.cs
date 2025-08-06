using Internship.Blazor.Exceptions;
using Microsoft.AspNetCore.Components;

namespace Internship.Blazor.Components;

public abstract class BaseComponent : ComponentBase, IDisposable
{
    [Inject] protected ILogger<BaseComponent> Logger { get; set; } = default!;
    
    protected bool ShowErrorDialog { get; set; } = false;
    protected string ErrorTitle { get; set; } = string.Empty;
    protected string ErrorMessage { get; set; } = string.Empty;
    protected Dictionary<string, string[]>? ValidationErrors { get; set; }

    protected Task HandleError(ApiException<ProblemDetails> ex)
    {
        Logger.LogError(ex, "API Error occurred: {StatusCode}", ex.StatusCode);

        var problemDetails = ex.ErrorData;
        
        ErrorTitle = problemDetails?.Title ?? "Error";
        ErrorMessage = problemDetails?.Detail ?? ex.Message;
        
        if (problemDetails?.Errors?.Any() == true)
        {
            ValidationErrors = problemDetails.Errors;
            ErrorTitle = "Validation Error";
            ErrorMessage = "Please correct the following errors:";
        }
        else
        {
            ValidationErrors = null;
        }

        ShowErrorDialog = true;
        StateHasChanged();
        return Task.CompletedTask;
    }

    protected void HandleError(ApiException ex)
    {
        Logger.LogError(ex, "API Error occurred: {StatusCode}", ex.StatusCode);

        ErrorTitle = GetTitleForStatusCode(ex.StatusCode);
        ErrorMessage = ex.Message;
        ValidationErrors = null;
        
        ShowErrorDialog = true;
        StateHasChanged();
    }

    private static string GetTitleForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden", 
            404 => "Not Found",
            409 => "Conflict",
            422 => "Validation Error",
            500 => "Server Error",
            0 => "Connection Error",
            408 => "Timeout",
            _ => "Error"
        };
    }

    protected void CloseErrorDialog()
    {
        ShowErrorDialog = false;
        ValidationErrors = null;
        StateHasChanged();
    }

    public virtual void Dispose()
    {
        // Override in derived classes if needed
    }
}
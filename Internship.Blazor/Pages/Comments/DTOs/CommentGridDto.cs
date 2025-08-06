namespace Internship.Blazor.Pages.Comments.DTOs;

public class CommentGridDto
{
    public Guid Id { get; set; }
    public string ContentPreview { get; set; } = string.Empty; 
    public DateTime CreatedAt { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string PostTitle { get; set; } = string.Empty;
}
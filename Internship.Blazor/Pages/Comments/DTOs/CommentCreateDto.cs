namespace Internship.Blazor.Pages.Comments.DTOs;

public class CommentCreateDto
{
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid PostId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
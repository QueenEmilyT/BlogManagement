namespace Internship.Application.Comments.DTOs;

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid PostId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? ParentCommentId { get; set; }
}

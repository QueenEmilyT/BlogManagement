namespace Internship.Application.Comments.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsEdited => UpdatedAt.HasValue;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorProfilePictureUrl { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    
}

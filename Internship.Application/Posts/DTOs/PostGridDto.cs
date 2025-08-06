namespace Internship.Application.Posts.DTOs;

public class PostGridDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool IsArchived { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string ExcerptPreview { get; set; } = string.Empty;
}

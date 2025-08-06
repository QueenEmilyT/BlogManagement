namespace Internship.Application.Posts.DTOs;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool IsArchived { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
}
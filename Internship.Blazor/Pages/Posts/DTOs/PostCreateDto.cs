using Internship.Blazor.Pages.Tags.DTOs;

namespace Internship.Blazor.Pages.Posts.DTOs;

public record PostCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public List<Guid> Tags { get; set; } = [];
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public DateTime? ScheduledPublishDate { get; set; }
    public bool PublishImmediately { get; set; } = false;
    public bool IsDraft { get; set; } = false;
}
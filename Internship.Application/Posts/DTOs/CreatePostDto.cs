using Internship.Application.Tags.DTOs;

namespace Internship.Application.Posts.DTOs;

public class CreatePostDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public List<Guid> TagIds { get; set; } = [];
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public DateTime? ScheduledPublishDate { get; set; }
    public bool PublishImmediately { get; set; } = false;
}

using Internship.Domain.Tags;

namespace Internship.Domain.Posts;

public class Post
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ArchivedAt { get; private set; }
    public bool IsPublished => PublishedAt.HasValue && PublishedAt.Value <= DateTime.UtcNow && !IsArchived;
    public bool IsArchived => ArchivedAt.HasValue && ArchivedAt.Value <= DateTime.UtcNow;
    public Guid AuthorId { get; private set; }
    public virtual ICollection<Tag> Tags { get; private set; } = [];
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public DateTime? ScheduledPublishDate { get; set; }
    public string Excerpt { get; set; } = string.Empty;
    
    // For EF Core
    private Post() { }
    
    public Post(Guid id, string title, string content, Guid authorId)
    {
        Id = id;
        Title = title;
        Content = content;
        AuthorId = authorId;
        CreatedAt = DateTime.UtcNow;
        Slug = GenerateSlug(title);
    }
    
    public void Update(string title, string content)
    {
        Title = title;
        Content = content;
        Slug = GenerateSlug(title);
    }
    
    public void Publish()
    {
        if (!IsArchived)
        {
            PublishedAt = DateTime.UtcNow;
        }
        else
        {
            throw new InvalidOperationException("Cannot publish an archived post");
        }
    }
    
    public void Archive()
    {
        ArchivedAt = DateTime.UtcNow;
    }
    
    public void Restore()
    {
        ArchivedAt = null;
    }
    
    
    public void ScheduleForPublishing(DateTime publishDate)
    {
        if (publishDate > DateTime.UtcNow)
        {
            ScheduledPublishDate = publishDate;
            Status = PostStatus.Scheduled;
        }
        else
        {
            throw new InvalidOperationException("Scheduled publish date must be in the future");
        }
    }
    
    public string GetExcerpt(int maxLength = 150)
    {
        if (!string.IsNullOrEmpty(Excerpt))
        {
            return Excerpt;
        }
        
        return Content.Length <= maxLength 
            ? Content 
            : string.Concat(Content.AsSpan(0, maxLength), "...");
    }
    
    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant();
        
        slug = slug.Replace(" ", "-");
        slug = new string(slug
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .ToArray());
        
        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }
        
        slug = slug.Trim('-');
        
        return slug;
    }
    
    public void AddTag(string tagName)
    {
        // This method will be used when we have tag names as strings
        // The actual Tag entity creation should be handled by the repository layer
        // For now, we'll store this as a placeholder - the repository will handle the conversion
    }
    
    public void AddTag(Tag tag)
    {
        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
        }
    }
}

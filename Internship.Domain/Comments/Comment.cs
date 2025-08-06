namespace Internship.Domain.Comments;

using Authors;
using Posts;

public sealed class Comment
{
    public Guid Id { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Relationships
    public Guid AuthorId { get; private set; }
    public Guid PostId { get; private set; }
    
    // Navigation properties (for EF Core)
    public Post Post { get; private set; }
    public Author Author { get; private set; }
    
    // Additional feature properties
    public int LikeCount { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ParentCommentId { get; set; }
    public bool IsEdited => UpdatedAt.HasValue;
    public int ReportCount { get; set; }
    public string IPAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    
    // Navigation properties for replies
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public Comment ParentComment { get; set; }
    
    // For EF Core
    private Comment() { }
    
    public Comment(Guid id, string content, Guid authorId, Guid postId)
    {
        Id = id;
        Content = content;
        AuthorId = authorId;
        PostId = postId;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update(string content)
    {
        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Like()
    {
        LikeCount++;
    }
    
    public void Unlike()
    {
        if (LikeCount > 0)
        {
            LikeCount--;
        }
    }
    
    
    public Comment AddReply(Guid id, string content, Guid authorId)
    {
        var reply = new Comment(id, content, authorId, PostId)
        {
            ParentCommentId = this.Id,
        };
        
        Replies.Add(reply);
        return reply;
    }
}
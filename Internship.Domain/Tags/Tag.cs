namespace Internship.Domain.Tags;

using Posts;

public class Tag
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    
    // Navigation property (for EF Core)
    public virtual ICollection<Post> Posts { get; private set; } = new List<Post>();
    
    // For EF Core
    private Tag() { }
    
    public Tag(Guid id, string name)
    {
        Id = id;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
    
    public Tag(Guid id, string name, string description) : this(id, name)
    {
        Description = description;
    }
    
    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

namespace Internship.Domain.Authors;

using Comments;
using Posts;

public sealed class Author
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Bio { get; private set; } = string.Empty;
    public DateTime JoinedAt { get; private set; }
    
    // Navigation properties (for EF Core)
    public ICollection<Post> Posts { get; private set; } = new List<Post>();
    public ICollection<Comment> Comments { get; private set; } = new List<Comment>();
    
    // For EF Core
    private Author() { }
    
    public Author(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
        JoinedAt = DateTime.UtcNow;
    }
    
    public void Update(string name, string email, string bio)
    {
        Name = name;
        Email = email;
        Bio = bio;
    }
    
    public void UpdateBio(string bio)
    {
        Bio = bio;
    }
    
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string TwitterHandle { get; set; } = string.Empty;
    public string FacebookUrl { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    
    public void UpdateProfilePicture(string url)
    {
        ProfilePictureUrl = url;
    }
    
    public void UpdateSocialLinks(string twitter = null, string facebook = null, string linkedin = null, string github = null)
    {
        if (twitter != null) TwitterHandle = twitter;
        if (facebook != null) FacebookUrl = facebook;
        if (linkedin != null) LinkedInUrl = linkedin;
        if (github != null) GitHubUrl = github;
    }
    
}

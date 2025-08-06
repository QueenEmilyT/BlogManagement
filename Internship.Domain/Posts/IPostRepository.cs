namespace Internship.Domain.Posts;

public interface IPostRepository
{
    // Basic CRUD operations
    Task<Post?> GetByIdAsync(Guid id);
    Task<Post?> GetBySlugAsync(string slug);
    Task<IEnumerable<Post>> GetAllAsync();
    Task<IEnumerable<Post>> GetByAuthorIdAsync(Guid authorId);
    Task<IEnumerable<Post>> GetPublishedAsync();
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
    Task<bool> DeleteAsync(Guid id);
    
    // Filtering operations
    Task<IEnumerable<Post>> SearchAsync(string query);
    Task<IEnumerable<Post>> GetArchivedAsync();
    
    // Aggregation operations
    Task<int> GetTotalCountAsync();
    Task<int> GetPublishedCountAsync();
    Task<int> GetArchivedCountAsync();
    Task<int> GetAuthorPostCountAsync(Guid authorId);
    
}
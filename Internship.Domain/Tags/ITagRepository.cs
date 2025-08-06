namespace Internship.Domain.Tags;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(Guid id);
    Task<Tag?> GetByNameAsync(string name);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<Tag> CreateAsync(Tag tag);
    Task<Tag> UpdateAsync(Tag tag);
    Task<bool> DeleteAsync(Guid id);
    
    Task<IEnumerable<Tag>> GetTagsByPostIdAsync(Guid postId);
    Task AddTagToPostAsync(Guid tagId, Guid postId);
    Task RemoveTagFromPostAsync(Guid tagId, Guid postId);
}

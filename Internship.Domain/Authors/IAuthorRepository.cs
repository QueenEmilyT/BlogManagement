namespace Internship.Domain.Authors;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(Guid id);
    Task<Author?> GetByEmailAsync(string email);
    Task<IEnumerable<Author>> GetAllAsync();
    Task<Author> CreateAsync(Author author);
    Task<Author> UpdateAsync(Author author);
    Task<bool> DeleteAsync(Guid id);
    
    // Search operations
    Task<IEnumerable<Author>> SearchAsync(string query);
   
}

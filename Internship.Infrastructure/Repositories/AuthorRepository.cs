using Internship.Domain.Authors;
using Microsoft.EntityFrameworkCore;

namespace Internship.Infrastructure.Repositories;

public class AuthorRepository(InternContext context) : IAuthorRepository
{
    public async Task<Author?> GetByIdAsync(Guid id)
    {
        return await context.Authors.FindAsync(id);
    }

    public async Task<Author?> GetByEmailAsync(string email)
    {
        return await context.Authors
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<IEnumerable<Author>> GetAllAsync()
    {
        return await context.Authors
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Author> CreateAsync(Author author)
    {
        await context.Authors.AddAsync(author);
        await context.SaveChangesAsync();
        return author;
    }

    public async Task<Author> UpdateAsync(Author author)
    {
        context.Authors.Update(author);
        await context.SaveChangesAsync();
        return author;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var author = await context.Authors.FindAsync(id);
        if (author == null) return false;
        
        context.Authors.Remove(author);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Author>> SearchAsync(string query)
    {
        query = query.ToLower();
        return await context.Authors
            .Where(a => a.Name.ToLower().Contains(query) || 
                       a.Email.ToLower().Contains(query) || 
                       (a.Bio != null && a.Bio.ToLower().Contains(query)))
            .OrderBy(a => a.Name)
            .ToListAsync();
    }
}

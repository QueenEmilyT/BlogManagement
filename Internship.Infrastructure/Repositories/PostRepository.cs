using Internship.Domain.Posts;
using Microsoft.EntityFrameworkCore;

namespace Internship.Infrastructure.Repositories;

public class PostRepository(InternContext context) : IPostRepository
{
    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await context.Posts.FindAsync(id);
    }

    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await context.Posts
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByAuthorIdAsync(Guid authorId)
    {
        return await context.Posts
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPublishedAsync()
    {
        return await context.Posts
            .Where(p => p.PublishedAt.HasValue && p.PublishedAt.Value <= DateTime.UtcNow && (!p.ArchivedAt.HasValue || p.ArchivedAt.Value > DateTime.UtcNow))
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<Post> CreateAsync(Post post)
    {
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
        return post;
    }

    public async Task<Post> UpdateAsync(Post post)
    {
        context.Posts.Update(post);
        await context.SaveChangesAsync();
        return post;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var post = await context.Posts.FindAsync(id);
        if (post == null) return false;
        
        context.Posts.Remove(post);
        await context.SaveChangesAsync();
        return true;
    }

    
    public async Task<IEnumerable<Post>> SearchAsync(string query)
    {
        query = query.ToLower();
        return await context.Posts
            .Where(p => p.Title.ToLower().Contains(query) || 
                       p.Content.ToLower().Contains(query) ||
                       (p.Excerpt != null && p.Excerpt.ToLower().Contains(query)))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetArchivedAsync()
    {
        return await context.Posts
            .Where(p => p.ArchivedAt.HasValue && p.ArchivedAt.Value <= DateTime.UtcNow)
            .OrderByDescending(p => p.ArchivedAt)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await context.Posts.CountAsync();
    }

    public async Task<int> GetPublishedCountAsync()
    {
        return await context.Posts
            .CountAsync(p => p.PublishedAt.HasValue && p.PublishedAt.Value <= DateTime.UtcNow && (!p.ArchivedAt.HasValue || p.ArchivedAt.Value > DateTime.UtcNow));
    }

    public async Task<int> GetArchivedCountAsync()
    {
        return await context.Posts
            .CountAsync(p => p.ArchivedAt.HasValue && p.ArchivedAt.Value <= DateTime.UtcNow);
    }

    public async Task<int> GetAuthorPostCountAsync(Guid authorId)
    {
        return await context.Posts
            .CountAsync(p => p.AuthorId == authorId);
    }

}
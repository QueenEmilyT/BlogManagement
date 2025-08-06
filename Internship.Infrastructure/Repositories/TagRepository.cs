using Internship.Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace Internship.Infrastructure.Repositories;

public class TagRepository(InternContext context) : ITagRepository
{
    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await context.Tags.FindAsync(id);
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await context.Tags
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await context.Tags
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await context.Tags
            .Where(t => ids.Contains(t.Id))
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
        await context.Tags.AddAsync(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateAsync(Tag tag)
    {
        context.Tags.Update(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tag = await context.Tags.FindAsync(id);
        if (tag == null) return false;
        
        context.Tags.Remove(tag);
        await context.SaveChangesAsync();
        return true;
    }


    public async Task<int> GetTotalCountAsync()
    {
        return await context.Tags.CountAsync();
    }

    public async Task<IEnumerable<Tag>> GetTagsByPostIdAsync(Guid postId)
    {
        var post = await context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            return [];

        return post.Tags;
    }

    public async Task AddTagToPostAsync(Guid tagId, Guid postId)
    {
        var tag = await context.Tags.FindAsync(tagId);
        var post = await context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (tag == null || post == null)
            return;

        // Use EF Core navigation properties for many-to-many relationship
        if (!post.Tags.Contains(tag))
        {
            post.Tags.Add(tag);
            await context.SaveChangesAsync();
        }
    }

    public async Task RemoveTagFromPostAsync(Guid tagId, Guid postId)
    {
        var post = await context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            return;

        var tagToRemove = post.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tagToRemove != null)
        {
            post.Tags.Remove(tagToRemove);
            await context.SaveChangesAsync();
        }
    }
}

using Internship.Domain.Comments;
using Microsoft.EntityFrameworkCore;

namespace Internship.Infrastructure.Repositories;

public class CommentRepository(InternContext context) : ICommentRepository
{
    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await context.Comments
            .Include(c => c.Author)
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId)
    {
        return await context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.Author)
            .Include(c => c.Replies)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetByAuthorIdAsync(Guid authorId)
    {
        return await context.Comments
            .Where(c => c.AuthorId == authorId)
            .Include(c => c.Post)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment> UpdateAsync(Comment comment)
    {
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var comment = await context.Comments.FindAsync(id);
        if (comment == null) return false;
        
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCommentCountByPostAsync(Guid postId)
    {
        return await context.Comments
            .CountAsync(c => c.PostId == postId);
    }

    public async Task<int> GetCommentCountByAuthorAsync(Guid authorId)
    {
        return await context.Comments
            .CountAsync(c => c.AuthorId == authorId);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await context.Comments.CountAsync();
    }

    public async Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count = 10)
    {
        return await context.Comments
            .Include(c => c.Author)
            .Include(c => c.Post)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetRepliesAsync(Guid parentCommentId)
    {
        return await context.Comments
            .Where(c => c.ParentCommentId == parentCommentId)
            .Include(c => c.Author)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}

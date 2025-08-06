namespace Internship.Domain.Comments;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId);
    Task<IEnumerable<Comment>> GetByAuthorIdAsync(Guid authorId);
    Task<Comment> CreateAsync(Comment comment);
    Task<Comment> UpdateAsync(Comment comment);
    Task<bool> DeleteAsync(Guid id);
    
    Task<int> GetCommentCountByPostAsync(Guid postId);
    Task<int> GetCommentCountByAuthorAsync(Guid authorId);
    Task<int> GetTotalCountAsync();
    Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count = 10);
    Task<IEnumerable<Comment>> GetRepliesAsync(Guid parentCommentId);
}

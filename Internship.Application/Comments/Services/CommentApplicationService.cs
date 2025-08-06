using Internship.Application.Comments.DTOs;
using Internship.Domain.Comments;
using Internship.Domain.Posts;
using Internship.Domain.Authors;

namespace Internship.Application.Comments.Services;

public class CommentApplicationService(
    ICommentRepository commentRepository,
    IPostRepository postRepository,
    IAuthorRepository authorRepository)
{
    // GET operations
    public async Task<CommentDto?> GetCommentByIdAsync(Guid id)
    {
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment == null)
            return null;

        var author = await authorRepository.GetByIdAsync(comment.AuthorId);
        var post = await postRepository.GetByIdAsync(comment.PostId);
        
        return await MapToCommentDtoAsync(comment, author?.Name ?? "Unknown Author", 
            author?.ProfilePictureUrl ?? string.Empty, post?.Title ?? "Unknown Post");
    }
    
    public async Task<IEnumerable<CommentGridDto>> GetCommentsByPostIdAsync(Guid postId)
    {
        var comments = await commentRepository.GetByPostIdAsync(postId);
        var commentDtos = new List<CommentGridDto>();
        
        var post = await postRepository.GetByIdAsync(postId);
        string postTitle = post?.Title ?? "Unknown Post";
        
        foreach (var comment in comments)
        {
            var author = await authorRepository.GetByIdAsync(comment.AuthorId);
            var replyCount = comment.Replies?.Count ?? 0;
            
            commentDtos.Add(MapToCommentGridDto(comment, author?.Name ?? "Unknown Author", 
                author?.ProfilePictureUrl ?? string.Empty, postTitle, replyCount));
        }
        
        return commentDtos;
    }
    
    public async Task<IEnumerable<CommentGridDto>> GetCommentsByAuthorIdAsync(Guid authorId)
    {
        var comments = await commentRepository.GetByAuthorIdAsync(authorId);
        var commentDtos = new List<CommentGridDto>();
        
        var author = await authorRepository.GetByIdAsync(authorId);
        var authorName = author?.Name ?? "Unknown Author";
        var profilePictureUrl = author?.ProfilePictureUrl ?? string.Empty;
        
        foreach (var comment in comments)
        {
            var post = await postRepository.GetByIdAsync(comment.PostId);
            var replyCount = comment.Replies?.Count ?? 0;
            
            commentDtos.Add(MapToCommentGridDto(comment, authorName, profilePictureUrl, 
                post?.Title ?? "Unknown Post", replyCount));
        }
        
        return commentDtos;
    }
    
    public async Task<IEnumerable<CommentGridDto>> GetRecentCommentsAsync(int count = 10)
    {
        var comments = await commentRepository.GetRecentCommentsAsync(count);
        var commentDtos = new List<CommentGridDto>();
        
        foreach (var comment in comments)
        {
            var author = await authorRepository.GetByIdAsync(comment.AuthorId);
            var post = await postRepository.GetByIdAsync(comment.PostId);
            var replyCount = comment.Replies?.Count ?? 0;
            
            commentDtos.Add(MapToCommentGridDto(comment, author?.Name ?? "Unknown Author", 
                author?.ProfilePictureUrl ?? string.Empty, post?.Title ?? "Unknown Post", replyCount));
        }
        
        return commentDtos;
    }
    
   
    
    // GET replies
    public async Task<IEnumerable<CommentDto>> GetRepliesAsync(Guid parentCommentId)
    {
        var replies = await commentRepository.GetRepliesAsync(parentCommentId);
        var replyDtos = new List<CommentDto>();
        
        foreach (var reply in replies)
        {
            var author = await authorRepository.GetByIdAsync(reply.AuthorId);
            var post = await postRepository.GetByIdAsync(reply.PostId);
            
            replyDtos.Add(await MapToCommentDtoAsync(reply, author?.Name ?? "Unknown Author", 
                author?.ProfilePictureUrl ?? string.Empty, post?.Title ?? "Unknown Post"));
        }
        
        return replyDtos;
    }
    
    // CREATE operations
    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto)
    {
        // Check if the post exists
        var post = await postRepository.GetByIdAsync(createCommentDto.PostId);
        if (post == null)
            throw new ArgumentException($"Post with ID {createCommentDto.PostId} not found");
            
            
        // Create the comment
        var comment = new Comment(
            Guid.NewGuid(),
            createCommentDto.Content,
            createCommentDto.AuthorId,
            createCommentDto.PostId
        );
        
        if (createCommentDto.ParentCommentId.HasValue)
        {
            var parentComment = await commentRepository.GetByIdAsync(createCommentDto.ParentCommentId.Value);
            if (parentComment == null)
                throw new ArgumentException($"Parent comment with ID {createCommentDto.ParentCommentId} not found");
                
            comment.ParentCommentId = createCommentDto.ParentCommentId;
        }
        
        var createdComment = await commentRepository.CreateAsync(comment);
        var author = await authorRepository.GetByIdAsync(createdComment.AuthorId);
        
        return await MapToCommentDtoAsync(createdComment, author?.Name ?? "Unknown Author", 
            author?.ProfilePictureUrl ?? string.Empty, post.Title);
    }
    
    // UPDATE operations
    public async Task<CommentDto> UpdateCommentAsync(Guid id, string content)
    {
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment == null)
            return null;
            
        // Update the comment content
        comment.Update(content);
        
        var updatedComment = await commentRepository.UpdateAsync(comment);
        var author = await authorRepository.GetByIdAsync(updatedComment.AuthorId);
        var post = await postRepository.GetByIdAsync(updatedComment.PostId);
        
        return await MapToCommentDtoAsync(updatedComment, author?.Name ?? "Unknown Author", 
            author?.ProfilePictureUrl ?? string.Empty, post?.Title ?? "Unknown Post");
    }
    
    // DELETE operations
    public async Task<bool> DeleteCommentAsync(Guid id)
    {
        return await commentRepository.DeleteAsync(id);
    }
    
    
    // STATISTICS operations
    public async Task<int> GetCommentCountByPostAsync(Guid postId)
    {
        return await commentRepository.GetCommentCountByPostAsync(postId);
    }
    
    public async Task<int> GetCommentCountByAuthorAsync(Guid authorId)
    {
        return await commentRepository.GetCommentCountByAuthorAsync(authorId);
    }
    
    public async Task<int> GetTotalCommentCountAsync()
    {
        return await commentRepository.GetTotalCountAsync();
    }
    
    // Helper methods for mapping
    private async Task<CommentDto> MapToCommentDtoAsync(Comment comment, string authorName, string authorProfilePictureUrl, string postTitle)
    {
        var replies = new List<CommentDto>();
        
        if (comment.Replies != null && comment.Replies.Any())
        {
            foreach (var reply in comment.Replies)
            {
                var replyAuthor = await authorRepository.GetByIdAsync(reply.AuthorId);
                replies.Add(await MapToCommentDtoAsync(reply, replyAuthor?.Name ?? "Unknown Author", 
                    replyAuthor?.ProfilePictureUrl ?? string.Empty, postTitle));
            }
        }
        
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            AuthorId = comment.AuthorId,
            AuthorName = authorName,
            AuthorProfilePictureUrl = authorProfilePictureUrl,
            PostId = comment.PostId,
            PostTitle = postTitle,
            ParentCommentId = comment.ParentCommentId,
        };
    }
    
    private CommentGridDto MapToCommentGridDto(Comment comment, string authorName, string authorProfilePictureUrl, string postTitle, int replyCount)
    {
        return new CommentGridDto
        {
            Id = comment.Id,
            ContentPreview = GetContentPreview(comment.Content),
            CreatedAt = comment.CreatedAt,
            AuthorName = authorName,
            PostTitle = postTitle,
        };
    }
    
    private string GetContentPreview(string content, int maxLength = 100)
    {
        return content.Length <= maxLength ? content : content[..maxLength] + "...";
    }
}

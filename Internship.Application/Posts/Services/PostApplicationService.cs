using Internship.Application.Posts.DTOs;
using Internship.Domain.Posts;
using Internship.Domain.Comments;
using Internship.Domain.Authors;
using Internship.Domain.Tags;

namespace Internship.Application.Posts.Services;

public class PostApplicationService(
    IPostRepository postRepository,
    ICommentRepository commentRepository,
    IAuthorRepository authorRepository,
    ITagRepository tagRepository)
{
    public async Task<PostDto?> GetPostByIdAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post == null)
            return null;

        var author = await authorRepository.GetByIdAsync(post.AuthorId);
        var commentCount = await commentRepository.GetCommentCountByPostAsync(id);
        
        return MapToPostDto(post, author?.Name ?? "Unknown Author", commentCount);
    }

    public async Task<PostDto?> GetPostBySlugAsync(string slug)
    {
        var post = await postRepository.GetBySlugAsync(slug);
        if (post == null)
            return null;
            
        var author = await authorRepository.GetByIdAsync(post.AuthorId);
        var commentCount = await commentRepository.GetCommentCountByPostAsync(post.Id);
        
        return MapToPostDto(post, author?.Name ?? "Unknown Author", commentCount);
    }

    public async Task<IEnumerable<PostGridDto>> GetAllPostsAsync()
    {
        var posts = await postRepository.GetAllAsync();
        var postDtos = new List<PostGridDto>();
        
        foreach (var post in posts)
        {
            var author = await authorRepository.GetByIdAsync(post.AuthorId);
            var commentCount = await commentRepository.GetCommentCountByPostAsync(post.Id);
            
            postDtos.Add(MapToPostGridDto(post, author?.Name ?? "Unknown Author", commentCount));
        }
        
        return postDtos;
    }

    public async Task<IEnumerable<PostGridDto>> GetPublishedPostsAsync()
    {
        var posts = await postRepository.GetPublishedAsync();
        var postDtos = new List<PostGridDto>();
        
        foreach (var post in posts)
        {
            var author = await authorRepository.GetByIdAsync(post.AuthorId);
            var commentCount = await commentRepository.GetCommentCountByPostAsync(post.Id);
            
            postDtos.Add(MapToPostGridDto(post, author?.Name ?? "Unknown Author", commentCount));
        }
        
        return postDtos;
    }
    
    public async Task<IEnumerable<PostGridDto>> GetPostsByAuthorIdAsync(Guid authorId)
    {
        var posts = await postRepository.GetByAuthorIdAsync(authorId);
        var postDtos = new List<PostGridDto>();
        
        var author = await authorRepository.GetByIdAsync(authorId);
        string authorName = author?.Name ?? "Unknown Author";
        
        foreach (var post in posts)
        {
            var commentCount = await commentRepository.GetCommentCountByPostAsync(post.Id);
            postDtos.Add(MapToPostGridDto(post, authorName, commentCount));
        }
        
        return postDtos;
    }
    
    
    public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto)
    {
        var post = new Post(
            Guid.NewGuid(),
            createPostDto.Title,
            createPostDto.Content,
            createPostDto.AuthorId
        );
        
        // Handle tags efficiently with a single query
        if (createPostDto.TagIds != null && createPostDto.TagIds.Any())
        {
            var tags = await tagRepository.GetByIdsAsync(createPostDto.TagIds);
            foreach (var tag in tags)
            {
                post.Tags.Add(tag);
            }
        }
        
        post.FeaturedImageUrl = createPostDto.FeaturedImageUrl;
        post.Excerpt = createPostDto.Excerpt;
        
        // Handle publishing options
        if (createPostDto.PublishImmediately)
        {
            post.Publish();
            post.Status = PostStatus.Published;
        }
        else if (createPostDto.ScheduledPublishDate.HasValue)
        {
            post.ScheduleForPublishing(createPostDto.ScheduledPublishDate.Value);
        }
        else
        {
            post.Status = PostStatus.Draft;
        }
        
        var createdPost = await postRepository.CreateAsync(post);
        var author = await authorRepository.GetByIdAsync(createdPost.AuthorId);
        
        return MapToPostDto(createdPost, author?.Name ?? "Unknown Author", 0);
    }
    
    // UPDATE operations
    public async Task<PostDto?> UpdatePostAsync(Guid id, CreatePostDto updatePostDto)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post == null)
            return null;
            
        // Update basic properties
        post.Update(updatePostDto.Title, updatePostDto.Content);
        
        // Update tags efficiently with a single query
        post.Tags.Clear();
        if (updatePostDto.TagIds != null && updatePostDto.TagIds.Any())
        {
            var tags = await tagRepository.GetByIdsAsync(updatePostDto.TagIds);
            foreach (var tag in tags)
            {
                post.Tags.Add(tag);
            }
        }
        
        // Update additional properties
        post.FeaturedImageUrl = updatePostDto.FeaturedImageUrl;
        post.Excerpt = updatePostDto.Excerpt;
        
        // Handle publishing options
        if (updatePostDto.PublishImmediately && !post.IsPublished)
        {
            post.Publish();
            post.Status = PostStatus.Published;
        }
        else if (updatePostDto.ScheduledPublishDate.HasValue)
        {
            post.ScheduleForPublishing(updatePostDto.ScheduledPublishDate.Value);
        }
        
        var updatedPost = await postRepository.UpdateAsync(post);
        var author = await authorRepository.GetByIdAsync(updatedPost.AuthorId);
        var commentCount = await commentRepository.GetCommentCountByPostAsync(id);
        
        return MapToPostDto(updatedPost, author?.Name ?? "Unknown Author", commentCount);
    }
    
    // DELETE operations
    public async Task<bool> DeletePostAsync(Guid id)
    {
        return await postRepository.DeleteAsync(id);
    }
    
    // Publishing operations
    public async Task<PostDto?> PublishPostAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post == null)
            return null;
            
        post.Publish();
        post.Status = PostStatus.Published;
        
        var updatedPost = await postRepository.UpdateAsync(post);
        var author = await authorRepository.GetByIdAsync(updatedPost.AuthorId);
        var commentCount = await commentRepository.GetCommentCountByPostAsync(id);
        
        return MapToPostDto(updatedPost, author?.Name ?? "Unknown Author", commentCount);
    }
    
    public async Task<PostDto?> ArchivePostAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post == null)
            return null;
            
        post.Archive();
        post.Status = PostStatus.Archived;
        
        var updatedPost = await postRepository.UpdateAsync(post);
        var author = await authorRepository.GetByIdAsync(updatedPost.AuthorId);
        var commentCount = await commentRepository.GetCommentCountByPostAsync(id);
        
        return MapToPostDto(updatedPost, author?.Name ?? "Unknown Author", commentCount);
    }
    
    public async Task<PostDto?> RestorePostAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post == null)
            return null;
            
        post.Restore();
        
        var updatedPost = await postRepository.UpdateAsync(post);
        var author = await authorRepository.GetByIdAsync(updatedPost.AuthorId);
        var commentCount = await commentRepository.GetCommentCountByPostAsync(id);
        
        return MapToPostDto(updatedPost, author?.Name ?? "Unknown Author", commentCount);
    }
    
    // Statistics operations
    public async Task<int> GetTotalPostCountAsync()
    {
        return await postRepository.GetTotalCountAsync();
    }
    
    public async Task<int> GetPublishedPostCountAsync()
    {
        return await postRepository.GetPublishedCountAsync();
    }
    
    public async Task<int> GetArchivedPostCountAsync()
    {
        return await postRepository.GetArchivedCountAsync();
    }
    
    // Helper methods for mapping
    private PostDto MapToPostDto(Post post, string authorName, int commentCount)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Slug = post.Slug,
            CreatedAt = post.CreatedAt,
            IsPublished = post.IsPublished,
            IsArchived = post.IsArchived,
            AuthorId = post.AuthorId,
            AuthorName = authorName,
            Tags = post.Tags.Select(t => t.Name).ToList(),
            FeaturedImageUrl = post.FeaturedImageUrl,
            Excerpt = !string.IsNullOrEmpty(post.Excerpt) ? post.Excerpt : post.GetExcerpt(),
        };
    }
    
    private PostGridDto MapToPostGridDto(Post post, string authorName, int commentCount)
    {
        return new PostGridDto
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            CreatedAt = post.CreatedAt,
            PublishedAt = post.PublishedAt,
            IsPublished = post.IsPublished,
            IsArchived = post.IsArchived,
            Status = post.Status.ToString(),
            AuthorName = authorName,
            ExcerptPreview = GetExcerptPreview(post, 100),
        };
    }
    
    private string GetExcerptPreview(Post post, int maxLength)
    {
        if (!string.IsNullOrEmpty(post.Excerpt))
            return post.Excerpt.Length <= maxLength ? post.Excerpt : post.Excerpt.Substring(0, maxLength) + "...";
            
        return post.Content.Length <= maxLength ? post.Content : post.Content.Substring(0, maxLength) + "...";
    }
}
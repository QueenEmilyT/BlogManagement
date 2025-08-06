using Internship.Blazor.Pages.Comments.DTOs;
using Internship.Blazor.Services;

namespace Internship.Blazor.Pages.Comments;

public class CommentsService(ApiService apiService)
{
    private const string Endpoint = "v1/Comments";
    
    public async Task<List<CommentGridDto>> GetCommentsAsync(Guid postId)
        => await apiService.GetAsync<List<CommentGridDto>>($"v1/comments/post/{postId}");

    public async Task<CommentDto> AddCommentAsync(CommentCreateDto Comment)
        => await apiService.PostAsync<CommentCreateDto, CommentDto>(Endpoint, Comment);

    public async Task<CommentDto> GetCommentAsync(Guid CommentId)
        => await apiService.GetAsync<CommentDto>($"{Endpoint}/{CommentId}");
    
    public async Task<CommentDto> UpdateCommentAsync(Guid CommentId, CommentDto Comment)
        => await apiService.PutAsync($"{Endpoint}/{CommentId}", Comment);

    public async Task DeleteCommentAsync(Guid CommentId)
        => await apiService.DeleteAsync($"{Endpoint}/{CommentId}");
}
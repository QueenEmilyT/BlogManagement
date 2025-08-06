using Internship.Blazor.Pages.Posts.DTOs;
using Internship.Blazor.Services;

namespace Internship.Blazor.Pages.Posts.Services;

public class PostsService(ApiService apiService)
{
    private const string Endpoint = "v1/posts";
    
    public async Task<List<PostGridDto>> GetPostsAsync()
        => await apiService.GetAsync<List<PostGridDto>>(Endpoint);

    public async Task<List<PostGridDto>> GetPostsByAuthorAsync(Guid authorId)
        => await apiService.GetAsync<List<PostGridDto>>($"{Endpoint}/author/{authorId}");

    public async Task<PostDto> AddPostAsync(PostCreateDto Post)
        => await apiService.PostAsync<PostCreateDto, PostDto>(Endpoint, Post);

    public async Task<PostDto> GetPostAsync(Guid postId)
        => await apiService.GetAsync<PostDto>($"{Endpoint}/{postId}");
    
    public async Task<PostDto> UpdatePostAsync(Guid postId, PostDto Post)
        => await apiService.PutAsync($"{Endpoint}/{postId}", Post);

    public async Task DeletePostAsync(Guid postId)
        => await apiService.DeleteAsync($"{Endpoint}/{postId}");
}
using Internship.Blazor.Pages.Posts.DTOs;
using Internship.Blazor.Pages.Tags.DTOs;
using Internship.Blazor.Services;

namespace Internship.Blazor.Pages.Tags;

public class TagsService(ApiService apiService)
{
    private const string Endpoint = "v1/tags";
    
    public async Task<List<TagGridDto>> GetTagsAsync()
        => await apiService.GetAsync<List<TagGridDto>>(Endpoint);

    public async Task<List<TagGridDto>> GetTagsByPostAsync(Guid postId)
        => await apiService.GetAsync<List<TagGridDto>>($"{Endpoint}/post/{postId}");

    public async Task<TagDto> AddTagAsync(CreateTagDto tag)
        => await apiService.PostAsync<CreateTagDto, TagDto>(Endpoint, tag);

    public async Task<TagDto> GetTagAsync(Guid tagId)
        => await apiService.GetAsync<TagDto>($"{Endpoint}/{tagId}");
    
    public async Task<TagDto> UpdateTagAsync(Guid tagId, TagDto tag)
        => await apiService.PutAsync($"{Endpoint}/{tagId}", tag);

    public async Task DeleteTagAsync(Guid tagId)
        => await apiService.DeleteAsync($"{Endpoint}/{tagId}");
}
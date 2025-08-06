using Internship.Blazor.Pages.Authors.DTOs;
using Internship.Blazor.Pages.Posts.DTOs;
using Internship.Blazor.Services;

namespace Internship.Blazor.Pages.Authors;

public class AuthorsService(ApiService apiService)
{
    private const string Endpoint = "v1/authors";
    
    public async Task<List<AuthorGridDto>> GetAuthorsAsync()
        => await apiService.GetAsync<List<AuthorGridDto>>(Endpoint);

    public async Task<AuthorDto> AddAuthorAsync(CreateAuthorDto createAuthor)
        => await apiService.PostAsync<CreateAuthorDto, AuthorDto>(Endpoint, createAuthor);

    public async Task<AuthorDto> GetAuthorAsync(Guid authorId)
        => await apiService.GetAsync<AuthorDto>($"{Endpoint}/{authorId}");
    
    public async Task<AuthorDto> UpdateAuthorAsync(Guid authorId, AuthorDto author)
        => await apiService.PutAsync($"{Endpoint}/{authorId}", author);

    public async Task DeleteAuthorAsync(Guid authorId)
        => await apiService.DeleteAsync($"{Endpoint}/{authorId}");
}
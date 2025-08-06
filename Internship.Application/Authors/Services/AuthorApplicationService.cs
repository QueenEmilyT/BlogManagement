using Internship.Application.Authors.DTOs;
using Internship.Domain.Authors;
using Internship.Domain.Posts;
using Internship.Domain.Comments;

namespace Internship.Application.Authors.Services;

public class AuthorApplicationService(
    IAuthorRepository authorRepository,
    IPostRepository postRepository,
    ICommentRepository commentRepository)
{
    // GET operations
    public async Task<AuthorDto?> GetAuthorByIdAsync(Guid id)
    {
        var author = await authorRepository.GetByIdAsync(id);
        if (author == null)
            return null;

        var postCount = await postRepository.GetAuthorPostCountAsync(id);
        var commentCount = await commentRepository.GetCommentCountByAuthorAsync(id);

        return MapToAuthorDto(author, postCount, commentCount);
    }

    public async Task<AuthorDto?> GetAuthorByEmailAsync(string email)
    {
        var author = await authorRepository.GetByEmailAsync(email);
        if (author == null)
            return null;

        var postCount = await postRepository.GetAuthorPostCountAsync(author.Id);
        var commentCount = await commentRepository.GetCommentCountByAuthorAsync(author.Id);

        return MapToAuthorDto(author, postCount, commentCount);
    }


    public async Task<IEnumerable<AuthorGridDto>> GetAllAuthorsAsync()
    {
        var authors = await authorRepository.GetAllAsync();
        var authorDtos = new List<AuthorGridDto>();

        foreach (var author in authors)
        {
            authorDtos.Add(MapToAuthorGridDto(author));
        }
        return authorDtos;
    }


    public async Task<IEnumerable<AuthorGridDto>> SearchAuthorsAsync(string query)
    {
        var authors = await authorRepository.SearchAsync(query);
        var authorDtos = new List<AuthorGridDto>();

        foreach (var author in authors)
        {
            authorDtos.Add(MapToAuthorGridDto(author));
        }

        return authorDtos;
    }

    // CREATE operations
    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
    {
        // Check if email already exists
        var existingByEmail = await authorRepository.GetByEmailAsync(createAuthorDto.Email);
        if (existingByEmail != null)
            throw new InvalidOperationException($"Email '{createAuthorDto.Email}' is already in use");

        var author = new Author(
            Guid.NewGuid(),
            createAuthorDto.Name,
            createAuthorDto.Email
        );

        // Update Bio using the proper domain method
        author.UpdateBio(createAuthorDto.Bio);
        
        // Set additional properties that have public setters
        author.ProfilePictureUrl = createAuthorDto.ProfilePictureUrl;
        author.WebsiteUrl = createAuthorDto.WebsiteUrl;
        author.Location = createAuthorDto.Location;
        
        // Update social links using the domain method
        author.UpdateSocialLinks(
            createAuthorDto.TwitterHandle,
            createAuthorDto.FacebookUrl,
            createAuthorDto.LinkedInUrl,
            createAuthorDto.GitHubUrl
        );

        var createdAuthor = await authorRepository.CreateAsync(author);
        return MapToAuthorDto(createdAuthor, 0, 0);
    }

    // UPDATE operations
    public async Task<AuthorDto> UpdateAuthorAsync(Guid id, CreateAuthorDto updateAuthorDto)
    {
        var author = await authorRepository.GetByIdAsync(id);
        if (author == null)
            return null;

        // Check if email is changed and already exists
        if (author.Email != updateAuthorDto.Email)
        {
            var existingByEmail = await authorRepository.GetByEmailAsync(updateAuthorDto.Email);
            if (existingByEmail != null && existingByEmail.Id != id)
                throw new InvalidOperationException($"Email '{updateAuthorDto.Email}' is already in use");
        }


        // Update properties using the domain method
        author.Update(updateAuthorDto.Name, updateAuthorDto.Email, updateAuthorDto.Bio);
        
        // Set additional properties that have public setters
        author.ProfilePictureUrl = updateAuthorDto.ProfilePictureUrl;
        author.WebsiteUrl = updateAuthorDto.WebsiteUrl;
        author.Location = updateAuthorDto.Location;
        
        // Update social links using the domain method
        author.UpdateSocialLinks(
            updateAuthorDto.TwitterHandle,
            updateAuthorDto.FacebookUrl,
            updateAuthorDto.LinkedInUrl,
            updateAuthorDto.GitHubUrl
        );

        var updatedAuthor = await authorRepository.UpdateAsync(author);
        var postCount = await postRepository.GetAuthorPostCountAsync(id);
        var commentCount = await commentRepository.GetCommentCountByAuthorAsync(id);

        return MapToAuthorDto(updatedAuthor, postCount, commentCount);
    }

    // DELETE operations
    public async Task<bool> DeleteAuthorAsync(Guid id)
    {
        return await authorRepository.DeleteAsync(id);
    }

    // PROFILE operations
    public async Task<AuthorDto> UpdateProfilePictureAsync(Guid id, string url)
    {
        var author = await authorRepository.GetByIdAsync(id);
        if (author == null)
            return null;

        author = await authorRepository.GetByIdAsync(id); // Get fresh data

        var postCount = await postRepository.GetAuthorPostCountAsync(id);
        var commentCount = await commentRepository.GetCommentCountByAuthorAsync(id);

        return MapToAuthorDto(author, postCount, commentCount);
    }

    public async Task<AuthorDto> UpdateSocialLinksAsync(Guid id, string twitter, string facebook, string linkedin, string github)
    {
        var author = await authorRepository.GetByIdAsync(id);
        if (author == null)
            return null;

        author = await authorRepository.GetByIdAsync(id); // Get fresh data

        var postCount = await postRepository.GetAuthorPostCountAsync(id);
        var commentCount = await commentRepository.GetCommentCountByAuthorAsync(id);

        return MapToAuthorDto(author, postCount, commentCount);
    }


    // Helper methods for mapping
    private AuthorDto MapToAuthorDto(Author author, int postCount, int commentCount)
    {
        return new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            Email = author.Email,
            Bio = author.Bio,
            ProfilePictureUrl = author.ProfilePictureUrl,
            WebsiteUrl = author.WebsiteUrl,
            TwitterHandle = author.TwitterHandle,
            FacebookUrl = author.FacebookUrl,
            LinkedInUrl = author.LinkedInUrl,
            GitHubUrl = author.GitHubUrl,
            Location = author.Location,
            JoinedAt = author.JoinedAt,
        };
    }

    private AuthorGridDto MapToAuthorGridDto(Author author)
    {
        return new AuthorGridDto
        {
            Id = author.Id,
            Name = author.Name,
            Email = author.Email,
            JoinedAt = author.JoinedAt,
            Bio = author.Bio,
        };
    }
}

namespace Internship.Application.Authors.DTOs;

public class CreateAuthorDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string TwitterHandle { get; set; } = string.Empty;
    public string FacebookUrl { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
}

namespace Internship.Blazor.Pages.Authors.DTOs;

public record AuthorGridDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; private set; } = string.Empty;
}
namespace Internship.Application.Authors.DTOs;

public class AuthorGridDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public string Bio { get; set; } = string.Empty;
}

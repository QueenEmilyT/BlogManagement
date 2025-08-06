using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Internship.Domain.Authors;

namespace Internship.Infrastructure.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
        builder.HasIndex(e => e.Email).IsUnique();
        
        // Additional properties
        builder.Property(e => e.ProfilePictureUrl).HasMaxLength(500);
        builder.Property(e => e.WebsiteUrl).HasMaxLength(200);
        builder.Property(e => e.TwitterHandle).HasMaxLength(50);
        builder.Property(e => e.FacebookUrl).HasMaxLength(200);
        builder.Property(e => e.LinkedInUrl).HasMaxLength(200);
        builder.Property(e => e.GitHubUrl).HasMaxLength(200);
        builder.Property(e => e.Location).HasMaxLength(100);
        builder.Property(e => e.Bio).HasMaxLength(2000);
    }
}

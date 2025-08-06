using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Internship.Domain.Tags;
using Internship.Domain.Posts;

namespace Internship.Infrastructure.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(500);

        // Many-to-many relationship with Posts
        builder.HasMany<Post>()
            .WithMany()
            .UsingEntity(j => j.ToTable("PostTags"));
    }
}

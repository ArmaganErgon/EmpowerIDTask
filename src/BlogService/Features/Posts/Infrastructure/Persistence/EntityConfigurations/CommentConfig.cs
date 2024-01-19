using BlogService.Features.Posts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogService.Features.Posts.Infrastructure.Persistence.EntityConfigurations;

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id)
            .HasName("Comment_PrimaryKey");

        builder.Property(x => x.Id).ValueGeneratedNever();
    }
}

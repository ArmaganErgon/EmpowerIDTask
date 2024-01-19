using BlogService.Features.Posts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogService.Features.Posts.Infrastructure.Persistence.EntityConfigurations;

public class PostConfig : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id)
            .HasName("Posts_PrimaryKey");
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

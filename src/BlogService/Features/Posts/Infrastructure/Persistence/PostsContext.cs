using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace BlogService.Features.Posts.Infrastructure.Persistence;

public class PostsDbContext(DbContextOptions<PostsDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PostConfig());
        modelBuilder.ApplyConfiguration(new CommentConfig());
    }
}

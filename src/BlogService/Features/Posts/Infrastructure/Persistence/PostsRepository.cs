using BlogService.Common.Interfaces;
using BlogService.Features.Posts.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogService.Features.Posts.Infrastructure.Persistence;

public class PostsRepository(PostsDbContext dbContext) : IRepository<Post>
{
    private readonly PostsDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<int> AddAsync(Post entity, CancellationToken ct = default)
    {
        await dbContext.Posts.AddAsync(entity, ct);
        return await dbContext.SaveChangesAsync(ct);
    }

    public Task<Post?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return dbContext.Posts
            .Include(p => p.Comments)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}

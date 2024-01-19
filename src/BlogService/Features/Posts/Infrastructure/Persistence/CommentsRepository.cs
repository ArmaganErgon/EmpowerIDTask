using BlogService.Common.Interfaces;
using BlogService.Features.Posts.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogService.Features.Posts.Infrastructure.Persistence;

public class CommentsRepository(PostsDbContext dbContext) : IRepository<Comment>
{
    private readonly PostsDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<int> AddAsync(Comment entity, CancellationToken ct = default)
    {
        await dbContext.Comments.AddAsync(entity, ct);
        return await dbContext.SaveChangesAsync(ct);
    }

    public Task<Comment?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return dbContext.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}

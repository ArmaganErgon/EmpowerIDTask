namespace BlogService.Common.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<int> AddAsync(TEntity entity, CancellationToken ct = default);
}

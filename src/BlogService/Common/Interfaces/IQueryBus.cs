namespace BlogService.Common.Interfaces;

public interface IQueryBus
{
    Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken ct = default);
}
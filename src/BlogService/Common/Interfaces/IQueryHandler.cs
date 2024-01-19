namespace BlogService.Common.Interfaces;

public interface IQueryHandler<in TQuery, TQueryResult>
{
    Task<TQueryResult> Handle(TQuery query, CancellationToken ct = default);
}
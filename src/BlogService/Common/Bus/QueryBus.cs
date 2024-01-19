using BlogService.Common.Interfaces;

namespace BlogService.Common.Bus;

internal sealed class QueryBus(IServiceProvider serviceProvider) : IQueryBus
{
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken ct)
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();

        return handler.Handle(query, ct);
    }
}
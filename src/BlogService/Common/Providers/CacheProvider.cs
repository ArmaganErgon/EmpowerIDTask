using BlogService.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BlogService.Common.Providers;

public class CacheProvider(IDistributedCache cache) : ICacheProvider
{
    private readonly IDistributedCache cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public Task AddAsync(string key, string value, TimeSpan expiry, CancellationToken ct = default)
    {
        return cache.SetStringAsync(key, value, 
            new DistributedCacheEntryOptions 
            { 
                AbsoluteExpirationRelativeToNow = expiry
            }, ct);
    }

    public Task<string?> GetAsync(string key, CancellationToken ct = default)
    {
        return cache.GetStringAsync(key, ct);
    }
}

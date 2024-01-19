namespace BlogService.Common.Interfaces;

public interface ICacheProvider
{
    Task AddAsync(string key, string value, TimeSpan expiry, CancellationToken ct = default);

    Task<string?> GetAsync(string key, CancellationToken ct = default);
}

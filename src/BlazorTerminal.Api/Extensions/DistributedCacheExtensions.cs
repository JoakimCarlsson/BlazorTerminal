namespace BlazorTerminal.Api.Extensions;

public static class DistributedCacheExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public static async Task SetAsync<T>(
        this IDistributedCache distributedCache,
        string key,
        T value,
        DistributedCacheEntryOptions? options = default,
        CancellationToken cancellationToken = default
        )
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(value, JsonSerializerOptions);
        await distributedCache.SetAsync(key, json, options ?? new DistributedCacheEntryOptions(), cancellationToken);
    }
    
    public static async Task<T?> GetAsync<T>(
        this IDistributedCache distributedCache,
        string key,
        CancellationToken cancellationToken = default
        )
    {
        var json = await distributedCache.GetAsync(key, cancellationToken);
        return json is null ? default : JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
    }
}
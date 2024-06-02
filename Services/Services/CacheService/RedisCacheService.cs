using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Challengify.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetObjectAsync<T>(string key)
    {
        string? value = await _cache.GetStringAsync(key);
        if (value == null)
        {
            return default;
        }

        var deserializedValue = JsonSerializer.Deserialize<T>(value);
        if (deserializedValue == null)
        {
            return default;
        }

        return deserializedValue;

    }

    public async Task SetObjectAsync<T>(string key, T value, TimeSpan expirationTime)
    {
        string serializedValue = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedValue, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        });
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
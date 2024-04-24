using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
	private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetCacheData<T>(string key)
    {
        var jsonData = await _cache.GetStringAsync(key);

        if (jsonData is null)
            return default;

        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public async Task RemoveData(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task SetCachedData<T>(string key, T data, TimeSpan cacheDuration)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration
        };

        var jsonData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(key, jsonData, options);
    }
}


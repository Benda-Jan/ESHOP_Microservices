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

    public T? GetCacheData<T>(string key)
    {
        var jsonData = _cache.GetString(key);

        if (jsonData is null)
            return default;

        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public void RemoveData(string key)
    {
        throw new NotImplementedException();
    }

    public void SetCachedData<T>(string key, T data, TimeSpan cacheDuration)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration
        };

        var jsonData = JsonSerializer.Serialize(data);
        _cache.SetString(key, jsonData, options);
    }
}


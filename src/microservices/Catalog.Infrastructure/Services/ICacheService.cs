using System;
namespace Catalog.Infrastructure.Services;

public interface ICacheService
{
    Task<T?> GetCacheData<T>(string key);
    Task SetCachedData<T>(string key, T data, TimeSpan cacheDuration);
    Task RemoveData(string key);

}


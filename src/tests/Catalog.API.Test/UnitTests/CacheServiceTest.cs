using Catalog.Entities.DbSet;
using Catalog.Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Catalog.API.Test.UnitTests;

public class CacheServiceTest
{
    [Fact]
    public async Task GetCachedData_SetCachedData_Correct()
    {
        var sut = GetRedisCache();
        var id = Guid.NewGuid().ToString();
        string key = $"CatalogItem-{id}";
        var brandName = "Apple";
        var item = new CatalogBrand{ Name = brandName, Id = "idididid" }; 

        await sut.SetCachedData(key, item, new TimeSpan(0, 1, 0));
        var result = await sut.GetCacheData<CatalogBrand>(key);

        Assert.NotNull(result);
        Assert.Equal(brandName, result.Name);
    }

    [Fact]
    public async Task GetCachedData_NotExists()
    {
        var sut = GetRedisCache();
        var id = Guid.NewGuid().ToString();
        string key = $"CatalogItem-{id}";

        var result = await sut.GetCacheData<CatalogBrand>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveData_Correct()
    {
        var sut = GetRedisCache();
        var id = Guid.NewGuid().ToString();
        string key = $"CatalogItem-{id}";
        var brandName = "Apple";
        var item = new CatalogBrand{ Name = brandName, Id = "idididid" }; 

        await sut.SetCachedData(key, item, new TimeSpan(0, 1, 0));
        var result1 = await sut.GetCacheData<CatalogBrand>(key);
        await sut.RemoveData(key);
        var result2 = await sut.GetCacheData<CatalogBrand>(key);

        Assert.NotNull(result1);
        Assert.Null(result2);
        Assert.Equal(brandName, result1.Name);
    }



    private ICacheService GetRedisCache()
    {
        var opts = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache cache = new MemoryDistributedCache(opts);

        return new RedisCacheService(cache);
    }
}
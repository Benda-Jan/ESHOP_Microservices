using System;
using Catalog.Infrastructure.Data;
using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.Services;

namespace Catalog.API.Handlers;

public class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, CatalogItem?>
{
    private readonly CatalogContext _catalogContext;
    private readonly RedisCacheService _cache;

    public GetItemByIdHandler(CatalogContext catalogContext, RedisCacheService cache)
    {
        _catalogContext = catalogContext;
        _cache = cache;
    }

    public async Task<CatalogItem?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        string key = $"CatalogItem-{request.Id}";
        var cacheResult = _cache.GetCacheData<CatalogItem>(key);
        if (cacheResult is not null)
            return cacheResult;

        var databaseResult = await _catalogContext.CatalogItems.FindAsync(request.Id);
        if (databaseResult is not null)
            _cache.SetCachedData<CatalogItem>(key, databaseResult, new TimeSpan(0, 0, 10));

        return databaseResult;
    }
        

}


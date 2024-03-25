using System;
using Catalog.Infrastructure.Data;
using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.Services;
using Microsoft.EntityFrameworkCore;
using Catalog.Entities.Models;

namespace Catalog.API.Handlers;

public class GetItemsWithBrandHandler : IRequestHandler<GetItemsWithBrandQuery, PaginatedItemsViewModel<CatalogItem>>
{
    private readonly CatalogContext _catalogContext;
    private readonly ICacheService _cache;

    public GetItemsWithBrandHandler(CatalogContext catalogContext, ICacheService cache)
    {
        _catalogContext = catalogContext;
        _cache = cache;
    }

    public async Task<PaginatedItemsViewModel<CatalogItem>> Handle(GetItemsWithBrandQuery request, CancellationToken cancellationToken)
    {
        var totalItems = await _catalogContext.CatalogItems.LongCountAsync();

        //string key = $"CatalogItem-{request.Id}";
        //var cacheResult = _cache.GetCacheData<CatalogItem>(key);
        //if (cacheResult is not null)
        //    return cacheResult;

        var databaseResult = await _catalogContext.CatalogItems
            .OrderBy(x => x.Name)
            .Where(x => x.CatalogBrand.Name == request.BrandName)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        //if (databaseResult is not null)
        //    _cache.SetCachedData<CatalogItem>(key, databaseResult, new TimeSpan(0, 0, 10));

        return new PaginatedItemsViewModel<CatalogItem>(request.PageIndex, request.PageSize, totalItems, databaseResult);
    }
        

}


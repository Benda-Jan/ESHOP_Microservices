using System;
using System.Text.Json;
using Catalog.API.Services;
using Catalog.Entities.DbSet;
using Catalog.Entities.Dtos;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure;

public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogContext _catalogContext;
    private readonly ICacheService _cache;

    public CatalogRepository(CatalogContext catalogContext, ICacheService cache)
    {
        _catalogContext = catalogContext;
        _cache = cache;
    }

    public async Task<CatalogItem?> CreateItem(CatalogItemInputDto inputItem)
    {
        var alreadyExists = await _catalogContext.CatalogItems.AnyAsync(x => x.Name == inputItem.Name);
        if (alreadyExists)
            throw new Exception("Catalog item already exists");

        var catalogType = await _catalogContext.CatalogTypes.Where(x => x.Name == inputItem.CatalogTypeName).FirstOrDefaultAsync()
            ?? throw new Exception($"Catalog type {inputItem.CatalogTypeName} does not exist");

        var catalogBrand = await _catalogContext.CatalogBrands.Where(x => x.Name == inputItem.CatalogBrandName).FirstOrDefaultAsync()
            ?? throw new Exception($"Catalog brand {inputItem.CatalogBrandName} does not exist");

        var catalogItem = new CatalogItem
        {
            Id = Guid.NewGuid().ToString(),
            Name = inputItem.Name,
            Description = inputItem.Description,
            Price = inputItem.Price,
            PictureFilename = inputItem.PictureFilename,
            PictureUri = inputItem.PictureUri,
            CatalogTypeId = catalogType.Id,
            CatalogType = catalogType,
            CatalogBrandId = catalogBrand.Id,
            CatalogBrand = catalogBrand,
            AvailableStock = inputItem.AvailableStock,
            RestockThreshold = inputItem.RestockThreshold,
            MaxStockThreshold = inputItem.MaxStockThreshold,
            OnReorder = inputItem.OnReorder
        };

        await _catalogContext.AddAsync(catalogItem);
        var changes = await _catalogContext.SaveChangesAsync();

        return changes > 0 ? catalogItem : null;
    }

    public async Task<CatalogItem?> RemoveItem(string itemId)
    {
        var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(x => x.Id == itemId);

        if (catalogItem is not null)
        {
            _catalogContext.Remove(catalogItem);
            await _catalogContext.SaveChangesAsync();
        }
        return catalogItem;
    }

    public async Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetAllItems(int pageSize = 10, int pageIndex = 0)
    {
        var totalItems = await _catalogContext.CatalogItems.LongCountAsync();

        var items = await _catalogContext.CatalogItems
            .OrderBy(x => x.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToArrayAsync();

        return (pageIndex, pageSize, totalItems, items);
    }

    public async Task<CatalogItem?> GetItemById(string itemId)
    {
        string key = $"CatalogItem-{itemId}";
        var cacheResult = _cache.GetCacheData<CatalogItem>(key);
        if (cacheResult is not null)
            return cacheResult;

        var databaseResult = await _catalogContext.CatalogItems.FindAsync(itemId);
        if (databaseResult is not null)
            _cache.SetCachedData<CatalogItem>(key, databaseResult, new TimeSpan(0, 0, 10));

        return databaseResult;
    }

    public async Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsWithBrand(string brandName, int pageSize = 10, int pageIndex = 0)
    {
        var totalItems = await _catalogContext.CatalogItems.LongCountAsync();

        var items = await _catalogContext.CatalogItems
            .OrderBy(x => x.Name)
            .Where(x => x.CatalogBrand != null && x.CatalogBrand.Name == brandName)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

        return (pageIndex, pageSize, totalItems, items);
    }

    public async Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsWithType(string typeName, int pageSize = 10, int pageIndex = 0)
    {
        var totalItems = await _catalogContext.CatalogItems.LongCountAsync();

        var items = await _catalogContext.CatalogItems
            .OrderBy(x => x.Name)
            .Where(x => x.CatalogType != null && x.CatalogType.Name == typeName)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

        return (pageIndex, pageSize, totalItems, items);
    }

    public async Task<CatalogItem?> UpdateItem(string itemId, CatalogItemInputDto inputItem)
    {
        var alreadyExists = await _catalogContext.CatalogItems.AnyAsync(x => x.Name == inputItem.Name);
        if (!alreadyExists)
            throw new Exception("Catalog item does not exist");

        var catalogType = await _catalogContext.CatalogTypes.Where(x => x.Name == inputItem.CatalogTypeName).FirstOrDefaultAsync()
            ?? throw new Exception($"Catalog type {inputItem.CatalogTypeName} does not exist");

        var catalogBrand = await _catalogContext.CatalogBrands.Where(x => x.Name == inputItem.CatalogBrandName).FirstOrDefaultAsync()
            ?? throw new Exception($"Catalog brand {inputItem.CatalogBrandName} does not exist");

        var catalogItem = new CatalogItem
        {
            Id = itemId,
            Name = inputItem.Name,
            Description = inputItem.Description,
            Price = inputItem.Price,
            PictureFilename = inputItem.PictureFilename,
            PictureUri = inputItem.PictureUri,
            CatalogTypeId = catalogType.Id,
            CatalogType = catalogType,
            CatalogBrandId = catalogBrand.Id,
            CatalogBrand = catalogBrand,
            AvailableStock = inputItem.AvailableStock,
            RestockThreshold = inputItem.RestockThreshold,
            MaxStockThreshold = inputItem.MaxStockThreshold,
            OnReorder = inputItem.OnReorder
        };

        _catalogContext.Update(catalogItem);
        await _catalogContext.SaveChangesAsync();

        return catalogItem;
    }
}


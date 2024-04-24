using System.Linq.Expressions;
using Catalog.Entities.DbSet;
using Catalog.Entities.Dtos;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure;

public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogContext _catalogContext;
    private readonly ICacheService _cache;
    private readonly TimeSpan _cacheTimeSpan;

    public CatalogRepository(CatalogContext catalogContext, ICacheService cache)
    {
        _catalogContext = catalogContext;
        _cache = cache;
        _cacheTimeSpan = new TimeSpan(0, 0, 10);
    }

    public async Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetAllItems(int pageSize = 10, int pageIndex = 0)
    {
        var totalItems = await _catalogContext.CatalogItems.LongCountAsync();
        var items = await _catalogContext.CatalogItems
            .Include(x => x.CatalogBrand)
            .Include(x => x.CatalogType)
            .OrderBy(x => x.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToArrayAsync();

        return (pageIndex, pageSize, totalItems, items);
    }

    public Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsByBrand(string brandName, int pageSize = 10, int pageIndex = 0)
        =>  GetItemsByProperty(x => x.CatalogBrand != null && x.CatalogBrand.Name == brandName, pageSize, pageIndex);

    public Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsByType(string typeName, int pageSize = 10, int pageIndex = 0)
        =>  GetItemsByProperty(x => x.CatalogType != null && x.CatalogType.Name == typeName, pageSize, pageIndex);

    public async Task<CatalogItem?> GetItemById(string itemId)
    {
        string key = $"CatalogItem-{itemId}";
        var cacheResult = await _cache.GetCacheData<CatalogItem>(key);
        if (cacheResult is not null)
            return cacheResult;

        var databaseResult = await _catalogContext.CatalogItems
            .Include(x => x.CatalogBrand)
            .Include(x => x.CatalogType)
            .SingleOrDefaultAsync(x => x.Id == itemId);
            
        if (databaseResult is not null)
            await _cache.SetCachedData(key, databaseResult, _cacheTimeSpan);

        return databaseResult;
    }

    public async Task<CatalogItem?> CreateItem(CatalogItemInputDto inputItem, string webRootPath)
    {
        if (await _catalogContext.CatalogItems.AnyAsync(x => x.Name == inputItem.Name))
            throw new Exception("Catalog item already exists");
        
        var catalogBrand = await GetBrandByName(inputItem.CatalogBrandName);
        var catalogType = await GetTypeByName(inputItem.CatalogTypeName);

        var filePath = await StoreImage(inputItem.File, webRootPath);

        var catalogItem = new CatalogItem
        {
            Id = Guid.NewGuid().ToString(),
            Name = inputItem.Name,
            Description = inputItem.Description,
            Price = inputItem.Price,
            PictureFilename = inputItem.File?.FileName ?? "",
            PictureUri = filePath ?? "",
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

    public async Task<CatalogBrand?> CreateBrand(string name)
    {
        if (await _catalogContext.CatalogBrands.AnyAsync(x => x.Name == name))
            throw new Exception($"Brand '{name}' already exists");

        var catalogBrand = new CatalogBrand() {Name = name};

        await _catalogContext.AddAsync(catalogBrand);
        var changes = await _catalogContext.SaveChangesAsync();

        return changes > 0 ? catalogBrand : null;
    }

    public async Task<CatalogType?> CreateType(string name)
    {
        if (await _catalogContext.CatalogTypes.AnyAsync(x => x.Name == name))
            throw new Exception($"Type '{name}' already exists");

        var catalogType = new CatalogType() {Name = name};

        await _catalogContext.AddAsync(catalogType);
        var changes = await _catalogContext.SaveChangesAsync();

        return changes > 0 ? catalogType : null;
    }

    public async Task<CatalogItem?> UpdateItem(string itemId, CatalogItemInputDto inputItem)
    {
        if (!await _catalogContext.CatalogItems.AnyAsync(x => x.Name == inputItem.Name))
            throw new Exception("Catalog item does not exist");

        var catalogBrand = await GetBrandByName(inputItem.CatalogBrandName);
        var catalogType = await GetTypeByName(inputItem.CatalogTypeName);

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

        return await UpdateDatabaseAndCache(catalogItem);
    }

    public async Task<CatalogItem?> UpdateItem(CatalogItem catalogItem)
    {
        if (!await _catalogContext.CatalogItems.AnyAsync(x => x.Name == catalogItem.Name))
            throw new Exception("Catalog item does not exist");

        return await UpdateDatabaseAndCache(catalogItem);
    }

    public async Task<CatalogItem?> RemoveItem(string itemId)
    {
        var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(x => x.Id == itemId)
            ?? throw new Exception("Catalog item does not exist");

        _catalogContext.Remove(catalogItem);
        await _catalogContext.SaveChangesAsync();

        string key = $"CatalogItem-{catalogItem.Id}";
        await _cache.RemoveData(key);
        
        return catalogItem;
    }

    public async Task<CatalogBrand?> RemoveBrand(string name)
    {
        var catalogBrand = await GetBrandByName(name);

        _catalogContext.Remove(catalogBrand);
        await _catalogContext.SaveChangesAsync();

        return catalogBrand;
    }

    public async Task<CatalogType?> RemoveType(string name)
    {
        var catalogType = await GetTypeByName(name);

        _catalogContext.Remove(catalogType);
        await _catalogContext.SaveChangesAsync();

        return catalogType;
    }

    private async Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsByProperty(Expression<Func<CatalogItem, bool>> filter, int pageSize = 10, int pageIndex = 0)
    {
        var totalItems = await _catalogContext.CatalogItems.LongCountAsync();
        var items = await _catalogContext.CatalogItems
            .Include(x => x.CatalogBrand)
            .Include(x => x.CatalogType)
            .OrderBy(x => x.Name)
            .Where(filter)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

        return (pageIndex, pageSize, totalItems, items);
    }

    private async Task<CatalogBrand> GetBrandByName(string brandName)
        => await _catalogContext.CatalogBrands.Where(x => x.Name == brandName).FirstOrDefaultAsync()
            ?? throw new Exception($"Catalog brand '{brandName}' does not exist");

    private async Task<CatalogType> GetTypeByName(string typeName)
        => await _catalogContext.CatalogTypes.Where(x => x.Name == typeName).FirstOrDefaultAsync()
            ?? throw new Exception($"Catalog type '{typeName}' does not exist");

    private async Task<CatalogItem?> UpdateDatabaseAndCache(CatalogItem catalogItem)
    {
        _catalogContext.Update(catalogItem);
        await _catalogContext.SaveChangesAsync();

        string key = $"CatalogItem-{catalogItem.Id}";
        await _cache.SetCachedData(key, catalogItem, _cacheTimeSpan);

        return catalogItem;
    }

    private async Task<string?> StoreImage(IFormFile? file, string webRootPath)
    {
        if (file == null || file.Length == 0)
            return null;

        try 
        {
            var _imagesFolderPath = Path.Combine(webRootPath, "images");
            var filePath = Path.Combine(_imagesFolderPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);
            return filePath;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}


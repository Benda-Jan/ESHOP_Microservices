using System;
using System.Text.Json;
using Catalog.Entities.DbSet;
using Catalog.Entities.Dtos;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure;

public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogContext _catalogContext;

    public CatalogRepository(CatalogContext catalogContext)
    {
        _catalogContext = catalogContext;
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

    public CatalogItem[] GetAllItems(int pageSize = 10, int pageIndex = 0)
    {
        throw new NotImplementedException();
    }

    public CatalogItem GetItemById(string itemId)
    {
        throw new NotImplementedException();
    }

    public CatalogItem[] GetItemsWithType(CatalogBrand brand)
    {
        throw new NotImplementedException();
    }

    public CatalogItem[] GetItemsWithType(CatalogType type)
    {
        throw new NotImplementedException();
    }

    public bool UpdateItem(CatalogItem item)
    {
        throw new NotImplementedException();
    }
}


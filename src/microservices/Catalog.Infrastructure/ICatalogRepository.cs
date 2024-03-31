using System;
using Catalog.Entities.DbSet;
using Catalog.Entities.Dtos;

namespace Catalog.Infrastructure;

public interface ICatalogRepository
{
    CatalogItem[] GetAllItems(int pageSize = 10, int pageIndex = 0);
    CatalogItem[] GetItemsWithType(CatalogBrand brand);
    CatalogItem[] GetItemsWithType(CatalogType type);
    CatalogItem GetItemById(string itemId);
    Task<CatalogItem?> CreateItem(CatalogItemInputDto inputItem);
    bool UpdateItem(CatalogItem item);
    Task<CatalogItem?> RemoveItem(string itemId);
}


using System;
using Catalog.Entities.DbSet;
using Catalog.Entities.Dtos;

namespace Catalog.Infrastructure;

public interface ICatalogRepository
{
    Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetAllItems(int pageSize = 10, int pageIndex = 0);
    Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsWithBrand(string brandName, int pageSize = 10, int pageIndex = 0);
    Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsWithType(string typeName, int pageSize = 10, int pageIndex = 0);
    Task<CatalogItem?> GetItemById(string itemId);
    Task<CatalogItem?> CreateItem(CatalogItemInputDto inputItem);
    Task<CatalogItem?> UpdateItem(string itemId, CatalogItemInputDto inputItem);
    Task<CatalogItem?> RemoveItem(string itemId);
}


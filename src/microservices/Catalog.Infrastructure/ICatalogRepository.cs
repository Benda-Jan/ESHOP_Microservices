using System;
using Catalog.Entities.DbSet;
using Catalog.Entities.Dtos;
using Microsoft.AspNetCore.Http;

namespace Catalog.Infrastructure;

public interface ICatalogRepository
{
    Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetAllItems(int pageSize = 10, int pageIndex = 0);
    Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsByBrand(string brandName, int pageSize = 10, int pageIndex = 0);
    Task<(int PageIndex, int PageSize, long TotalItems, CatalogItem[] Items)> GetItemsByType(string typeName, int pageSize = 10, int pageIndex = 0);
    Task<CatalogItem?> GetItemById(string itemId);
    Task<CatalogItem?> CreateItem(CatalogItemInputDto inputItem, string webRootPath);
    Task<CatalogBrand?> CreateBrand(string name);
    Task<CatalogType?> CreateType(string name);
    Task<CatalogItem?> UpdateItem(string itemId, CatalogItemInputDto inputItem);
    Task<CatalogItem?> UpdateItem(CatalogItem catalogItem);
    Task<CatalogItem?> RemoveItem(string itemId);
    Task<CatalogBrand?> RemoveBrand(string name);
    Task<CatalogType?> RemoveType(string name);
}


using System;
using Catalog.Entities.DbSet;
using Catalog.Entities.Models;
using MediatR;

namespace Catalog.API.Read.Queries;

public class GetItemsWithBrandQuery(string brandName, int pageSize, int pageIndex) : IRequest<PaginatedItemsViewModel<CatalogItem>>
{
    public string BrandName { get; } = brandName;
    public int PageSize { get; } = pageSize;
    public int PageIndex { get; } = pageIndex;
}


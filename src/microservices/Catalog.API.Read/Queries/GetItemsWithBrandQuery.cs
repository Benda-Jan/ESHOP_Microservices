using System;
using Catalog.Entities.DbSet;
using Catalog.Entities.Models;
using MediatR;

namespace Catalog.API.Read.Queries;

public class GetItemsWithBrandQuery : IRequest<PaginatedItemsViewModel<CatalogItem>>
{
    public string BrandName { get; }
    public int PageSize { get; }
    public int PageIndex { get; }

    public GetItemsWithBrandQuery(string brandName, int pageSize, int pageIndex)
    {
        BrandName = brandName;
        PageSize = pageSize;
        PageIndex = pageIndex;
    }
}

